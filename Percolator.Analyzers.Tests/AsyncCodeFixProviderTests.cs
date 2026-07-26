using System.Threading.Tasks;
using SampleCode;
using Xunit;

using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    Percolator.Analyzers.AsyncAnalyzer,
    Percolator.Analyzers.AsyncCodeFixProvider,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace Percolator.Analyzers.Tests
{
    public class AsyncCodeFixProviderTests
    {
        [Fact]
        public async Task ReturnTask_IsReplacedWithAwaitTask()
        {
            var includes = Sample.For<IGreeter>();

            // Lang=C#
            var text = includes + """

public class Greeter : IGreeter
{
    public Task {|#0:Greet|}() => Task.CompletedTask;
    public Task {|#1:Farewell|}() {
        return Task.CompletedTask;
    }
}
""";
            // Lang=C#
            var newText = includes + """

public class Greeter : IGreeter
{
    public async Task Greet() => await Task.CompletedTask;
    public async Task Farewell() {
        await Task.CompletedTask;
    }
}
""";
            await Verifier.VerifyCodeFixAsync(text, [
                Verifier.Diagnostic().WithLocation(0).WithArguments("Greet"),
                Verifier.Diagnostic().WithLocation(1).WithArguments("Farewell")
            ], newText).ConfigureAwait(false);
        }

        [Fact]
        public async Task ReturnTaskOf_IsReplacedWithReturnAwaitTaskOf()
        {
            var includes = Sample.For(typeof(IRepository<>));

            // Lang=C#
            var text = includes + """

public class CustomerRepository : IRepository<Customer>
{
    public Task<Customer> {|#0:Load|}() => Task.FromResult(new Customer(Guid.NewGuid()));
    public async Task Save() => await Task.CompletedTask;
}

public class OrderRepository : IRepository<Order>
{
    public Task<Order> {|#1:Load|}() {
        return Task.FromResult(new Order(Guid.NewGuid()));
    }

    public async Task Save() => await Task.CompletedTask;
}
""";
            // Lang=C#
            var newText = includes + """

public class CustomerRepository : IRepository<Customer>
{
    public async Task<Customer> Load() => await Task.FromResult(new Customer(Guid.NewGuid()));
    public async Task Save() => await Task.CompletedTask;
}

public class OrderRepository : IRepository<Order>
{
    public async Task<Order> Load() {
        return await Task.FromResult(new Order(Guid.NewGuid()));
    }

    public async Task Save() => await Task.CompletedTask;
}
""";
            await Verifier.VerifyCodeFixAsync(text, [
                Verifier.Diagnostic().WithLocation(0).WithArguments("Load"),
                Verifier.Diagnostic().WithLocation(1).WithArguments("Load")
            ], newText).ConfigureAwait(false);
        }
    }
}

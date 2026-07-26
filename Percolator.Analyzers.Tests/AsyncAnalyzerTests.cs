using System.Threading.Tasks;
using SampleCode;
using Xunit;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerVerifier<Percolator.Analyzers.AsyncAnalyzer, Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace Percolator.Analyzers.Tests;

public class AsyncAnalyzerTests
{
    [Fact]
    public async Task ReturnTask_IsFlagged()
    {
        var includes = Sample.For<IGreeter>();

        // Lang=C#
        var text = includes + """

public class ProperGreeter : IGreeter
{
    public async Task Greet() => await Task.CompletedTask;
    public async Task Farewell() {
        await Task.CompletedTask;
    }
}

public class SloppyGreeter : IGreeter
{
    public Task {|#0:Greet|}() => Task.CompletedTask;
    public Task {|#1:Farewell|}() {
        return Task.CompletedTask;
    }
}
""";
        await Verifier.VerifyAnalyzerAsync(text, [
            Verifier.Diagnostic().WithLocation(0).WithArguments("Greet"),
            Verifier.Diagnostic().WithLocation(1).WithArguments("Farewell")
        ]).ConfigureAwait(false);
    }


    [Fact]
    public async Task ReturnTaskOf_IsFlagged()
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

        await Verifier.VerifyAnalyzerAsync(text, [
            Verifier.Diagnostic().WithLocation(0).WithArguments("Load"),
            Verifier.Diagnostic().WithLocation(1).WithArguments("Load")
        ]).ConfigureAwait(false);
    }
}
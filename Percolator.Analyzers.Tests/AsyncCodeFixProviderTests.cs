using System.Threading.Tasks;
using Xunit;

using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<Percolator.Analyzers.AsyncAnalyzer,
        Percolator.Analyzers.AsyncCodeFixProvider>;

namespace Percolator.Analyzers.Tests
{
    public class AsyncCodeFixProviderTests
    {
        [Fact]
        public async Task ReturnTask_IsReplacedWithAwaitTask()
        {
            // Lang=C#
            const string includes = """
using System.Threading.Tasks;

namespace SampleCode;

public interface IGreeter
{
    Task Greet();
    Task Farewell();
}


""";
            // Lang=C#
            const string text = includes + """
public class Greeter : IGreeter
{
    public Task {|#0:Greet|}() => Task.CompletedTask;
    public Task {|#1:Farewell|}() {
        return Task.CompletedTask;
    }
}
""";
            // Lang=C#
            const string newText = includes + """
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
            // Lang=C#
            const string includes = """
using System;
using System.Threading.Tasks;

public class Customer
{
    public Guid Id { get; }
    public Customer(Guid id) => Id = id;
}

public class Order
{
    public Guid Id { get; }
    public Order(Guid id) => Id = id;
}

public interface IRepository<T>
{
    Task<T> Load();
}


""";
            // Lang=C#
            const string text = includes + """
public class CustomerRepository : IRepository<Customer>
{
    public Task<Customer> {|#0:Load|}() => Task.FromResult(new Customer(Guid.NewGuid()));
}

public class OrderRepository : IRepository<Order>
{
    public Task<Order> {|#1:Load|}() {
        return Task.FromResult(new Order(Guid.NewGuid()));
    }
}
""";
            // Lang=C#
            const string newText = includes + """
public class CustomerRepository : IRepository<Customer>
{
    public async Task<Customer> {|#0:Load|}() => await Task.FromResult(new Customer(Guid.NewGuid()));
}

public class OrderRepository : IRepository<Order>
{
    public async Task<Order> {|#1:Load|}() {
        return await Task.FromResult(new Order(Guid.NewGuid()));
    }
}
""";
            await Verifier.VerifyCodeFixAsync(text, [
                Verifier.Diagnostic().WithLocation(0).WithArguments("Load"),
                Verifier.Diagnostic().WithLocation(1).WithArguments("Load")
            ], newText).ConfigureAwait(false);
        }
    }
}
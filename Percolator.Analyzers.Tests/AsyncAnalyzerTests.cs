using System.Threading.Tasks;
using Xunit;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<Percolator.Analyzers.AsyncAnalyzer>;

namespace Percolator.Analyzers.Tests;

public class AsyncAnalyzerTests
{
    [Fact]
    public async Task TestAsyncAnalyzer()
    {
        // Lang=C#
        var text =
"""
using System.Threading.Tasks;

namespace SampleCode;

public interface IGreeter
{
    Task Greet();
    Task Farewell();
}

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
}
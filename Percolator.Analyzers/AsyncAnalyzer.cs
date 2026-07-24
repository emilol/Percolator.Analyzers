using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Percolator.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AsyncAnalyzer : DiagnosticAnalyzer
{
    // Preferred format of DiagnosticId is Your Prefix + Number, e.g. CA1234.
    public const string DiagnosticId = "PA00001";

    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.PA00001Title),
        Resources.ResourceManager, typeof(Resources));

    // The message that will be displayed to the user.
    private static readonly LocalizableString MessageFormat =
        new LocalizableResourceString(nameof(Resources.PA00001MessageFormat), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString Description =
        new LocalizableResourceString(nameof(Resources.PA00001Description), Resources.ResourceManager,
            typeof(Resources));

    // The category of the diagnostic (Design, Naming etc.).
    private const string Category = "Usage";

    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category,
        DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description,
        helpLinkUri: "https://github.com/emilol/Percolator.Analyzers/wiki/PA00001");

    // Keep in mind: you have to list your rules here.
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        // You must call this method to avoid analyzing generated code.
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        
        // You must call this method to enable the Concurrent Execution.
        if (!Debugger.IsAttached) context.EnableConcurrentExecution();

        // Subscribe to the Syntax Node with the appropriate 'SyntaxKind' (ClassDeclaration) action.
        // To figure out which Syntax Nodes you should choose, consider installing the Roslyn syntax tree viewer plugin Rossynt: https://plugins.jetbrains.com/plugin/16902-rossynt/
        context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.MethodDeclaration);

        // Check other 'context.Register...' methods that might be helpful for your purposes.
    }

    private void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;

        // Interface/abstract signatures have no implementation to make async; only flag real bodies.
        if (methodDeclaration.Body is null && methodDeclaration.ExpressionBody is null)
            return;

        if (methodDeclaration.Modifiers.Any(SyntaxKind.AsyncKeyword))
            return;

        if (!ReturnsTask(context.SemanticModel, methodDeclaration.ReturnType))
            return;

        var diagnostic = Diagnostic.Create(Rule, methodDeclaration.Identifier.GetLocation(), methodDeclaration.Identifier.Text);

        context.ReportDiagnostic(diagnostic);
    }

    private static bool ReturnsTask(SemanticModel semanticModel, TypeSyntax returnType)
    {
        if (semanticModel.GetTypeInfo(returnType).Type is not INamedTypeSymbol { ContainingNamespace: { } ns } type)
            return false;

        return ns.ToDisplayString() == "System.Threading.Tasks" && type.Name is "Task" or "ValueTask";
    }
}
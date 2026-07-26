using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace Percolator.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AsyncCodeFixProvider)), Shared]
public class AsyncCodeFixProvider : CodeFixProvider
{
    private const string Title = "Make method async";

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
            return;

        var diagnostic = context.Diagnostics.First();
        var methodDeclaration = root.FindNode(diagnostic.Location.SourceSpan)
            .FirstAncestorOrSelf<MethodDeclarationSyntax>();

        if (methodDeclaration is null)
            return;

        context.RegisterCodeFix(
            CodeAction.Create(
                Title,
                cancellationToken => MakeAsync(context.Document, methodDeclaration, cancellationToken),
                Title),
            diagnostic);
    }

    private static async Task<Document> MakeAsync(Document document, MethodDeclarationSyntax methodDeclaration,
        CancellationToken cancellationToken)
    {
        var asyncKeyword = SyntaxFactory.Token(SyntaxKind.AsyncKeyword).WithTrailingTrivia(SyntaxFactory.Space);
        var newMethod = methodDeclaration.AddModifiers(asyncKeyword);

        // Task<T> still has to return a value; only a bare Task can drop "return" entirely.
        var returnsValue = newMethod.ReturnType is GenericNameSyntax;

        newMethod = newMethod.ExpressionBody is { } expressionBody
            ? newMethod.WithExpressionBody(expressionBody.WithExpression(Await(expressionBody.Expression)))
            : newMethod.WithBody(newMethod.Body!.WithStatements(SyntaxFactory.List(
                newMethod.Body.Statements.Select(statement =>
                    statement is ReturnStatementSyntax { Expression: { } returnExpression } returnStatement
                        ? ConvertReturn(returnStatement, returnExpression, returnsValue)
                        : statement))));

        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        editor.ReplaceNode(methodDeclaration, newMethod);

        return editor.GetChangedDocument();
    }

    private static AwaitExpressionSyntax Await(ExpressionSyntax expression) =>
        SyntaxFactory.AwaitExpression(
            SyntaxFactory.Token(SyntaxKind.AwaitKeyword).WithTrailingTrivia(SyntaxFactory.Space),
            expression.WithoutLeadingTrivia());

    private static StatementSyntax ConvertReturn(ReturnStatementSyntax returnStatement, ExpressionSyntax returnExpression,
        bool returnsValue) =>
        returnsValue
            // Task<T>: keep "return", the value still has to come back out of the method.
            ? returnStatement.WithExpression(Await(returnExpression))
            // Bare Task: nothing to return, just await the work.
            : SyntaxFactory.ExpressionStatement(Await(returnExpression)).WithTriviaFrom(returnStatement);

    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(AsyncAnalyzer.DiagnosticId);

    public override FixAllProvider GetFixAllProvider()
        => WellKnownFixAllProviders.BatchFixer;
}

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MoreThanOneNamespaceInFileIsNotAllowedAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "MyRules0001";

    private static readonly LocalizableString Title = "This is a title";
    private static readonly LocalizableString MessageFormat = "This is a message";
    private static readonly LocalizableString Description = "This is a description";
    private const string Category = "Naming";
    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId,
    Title,
    MessageFormat,
    Category,
    DiagnosticSeverity.Error,
    true,
    Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxTreeAction(AnalyzeAction);
    }

    private static void AnalyzeAction(SyntaxTreeAnalysisContext context)
    {
        var syntaxRoot = context.Tree.GetRoot(context.CancellationToken);

        var descentNodes = syntaxRoot.
            DescendantNodes(node => node != null && !node.IsKind(SyntaxKind.ClassDeclaration));

        var foundNode = false;
        foreach (var node in descentNodes)
        {
            if (node.IsKind(SyntaxKind.NamespaceDeclaration))
            {
                if (foundNode)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, Location.None));
                }
                else
                {
                    foundNode = true;
                }
            }
        }
    }
}

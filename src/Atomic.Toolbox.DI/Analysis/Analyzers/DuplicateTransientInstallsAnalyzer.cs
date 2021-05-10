using System.Collections.Immutable;
using System.Linq;
using Atomic.Toolbox.Core.Parsers;
using Atomic.Toolbox.DI.Analyzers;
using Atomic.Toolbox.DI.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Atomic.Toolbox.DI.Analysis.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DuplicateTransientInstallsAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticID = AnalyzerID.DuplicateTransientInstalls;
        private const string Title = "Multiple Transient installs for the same type";
        private const string MessageFormat =
            "Multiple fields of the same type can't be registered as Transient. Consider adding another InstallTransient attribute to the same field with different ID.";
        private const string Category = "Declaration";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticID, Title, MessageFormat,
            Category, DiagnosticSeverity.Error, true);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                                   GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSemanticModelAction(AnalyzeTree);
        }

        private void AnalyzeTree(SemanticModelAnalysisContext context)
        {
            var tree = new Tree(context.SemanticModel);


            var classes = tree.GetClassesWithInterface(DiInterfaces.ContainerType);

            foreach (var @class in classes)
            {
                var fields = @class.GetFieldsWithAttributes(DiAttributes.TransientType);

                var diagnostics = fields
                    .GroupBy(f => f.TypeName)
                    .Select(g => g.ToList())
                    .Where(g => g.Count > 1)
                    .Select(g => g.First())
                    .Select(f => Diagnostic.Create(Rule, f.Location))
                    .ToList();

                diagnostics.ForEach(context.ReportDiagnostic);
            }
        }
    }
}
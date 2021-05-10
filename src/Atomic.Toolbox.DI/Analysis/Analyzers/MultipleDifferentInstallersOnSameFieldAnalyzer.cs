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
    public class MultipleDifferentInstallersOnSameFieldAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticID = AnalyzerID.MultipleDifferentInstallersOnSameField;
        private const string Title = "Multiple different Install Attributes";
        private const string MessageFormat = "Field cannot have different install attributes";
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
                var fields = @class.GetFieldsWithAttributes(DiAttributes.InstallTypes);

                var diagnostics = fields
                    .Where(field => field.Attributes.Count > 1)
                    .Where(field => field
                        .CountAttributes(DiAttributes.InstallTypes)
                        .Count(pair => pair.count > 0) > 1)
                    .Select(field => Diagnostic.Create(Rule, field.Location))
                    .ToList();

                diagnostics.ForEach(context.ReportDiagnostic);
            }
        }
    }
}
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
    public class MultipleTransientInstallsWithoutIDAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticID = AnalyzerID.MultipleTransientInstallsWithoutID;
        private const string Title = "Multiple transient installs for the same type without ID";
        private const string MessageFormat = "Single Transient type can't have multiple installations without an ID";
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

                var diagnostics = fields.Select(field => field
                        .GetAttributes(DiAttributes.TransientType)
                        .Where(attribute => !attribute.HasArgument(DiAttributes.Fields.ID))
                        .ToList())
                    .Where(attributes => attributes.Count > 1)
                    .Select(attributes => Diagnostic.Create(Rule, attributes.Last().Location))
                    .ToList();

                diagnostics.ForEach(context.ReportDiagnostic);
            }
        }
    }
}
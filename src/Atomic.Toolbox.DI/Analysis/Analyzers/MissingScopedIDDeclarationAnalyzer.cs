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
    public class MissingScopedIDDeclarationAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticID = AnalyzerID.MissingScopedIDDeclaration;
        private const string Title = "Missing Id for Scoped installation";
        private const string MessageFormat = "Scoped Installation must have 'ID' attribute";
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
                var fields = @class.GetFieldsWithAttributes(DiAttributes.ScopedType);

                var diagnostics = fields
                    .Select(field => field.GetAttributes(DiAttributes.ScopedType))
                    .SelectMany(scopedAttributes => scopedAttributes
                        .Where(attribute => !attribute.HasArgument(DiAttributes.Fields.ID)))
                    .Select(a => Diagnostic.Create(Rule, a.Location))
                    .ToList();

                diagnostics.ForEach(context.ReportDiagnostic);
            }
        }
    }
}
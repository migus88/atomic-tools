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
    public class ContainerConstructorTriggerAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticID = AnalyzerID.ContainerConstructorTrigger;
        private const string Title = "Container doesn't trigger internal constructor";
        private const string MessageFormat = "Container constructors must trigger internal constructors: ': this()'";
        private const string Category = "Initialization";

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
                var diagnostics = @class.Constructors
                    .Where(c => c.Parameters != null && c.Parameters.Count > 0 && !c.IsTriggeringInternalConstructor)
                    .Select(c => Diagnostic.Create(Rule, c.Location)).ToList();

                diagnostics.ForEach(context.ReportDiagnostic);
            }
        }
    }
}
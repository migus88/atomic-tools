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
    public class MultipleInjectConstructorsAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticID = AnalyzerID.MultipleInjectConstructors;
        private const string Title = "Only one Inject constructor allowed";
        private const string MessageFormat = "Only one constructor with inject attribute allowed";
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

            foreach (var @class in tree.Classes)
            {
                var injectAttributes = @class.Constructors
                    .Where(constructor => constructor.HasAttribute(DiAttributes.InjectType))
                    .Select(constructor => constructor.GetAttribute(DiAttributes.InjectType))
                    .ToList();

                if (injectAttributes.Count <= 1)
                {
                    continue;
                }
                
                injectAttributes.ForEach(attribute => context.ReportDiagnostic(Diagnostic.Create(Rule, attribute.Location)));
            }
        }
    }
}
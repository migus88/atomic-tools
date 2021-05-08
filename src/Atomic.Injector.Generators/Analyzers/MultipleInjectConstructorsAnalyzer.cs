using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Atomic.Generators.Tools.Parsers;
using Atomic.Injector.Generators.Helpers.Identifiers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static Atomic.Injector.Generators.Helpers.Identifiers.InstallAttributeTypes;

namespace Atomic.Injector.Generators.Analyzers
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
                    .Where(constructor => constructor.HasAttribute(InjectAttributeType))
                    .Select(constructor => constructor.GetAttribute(InjectAttributeType))
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
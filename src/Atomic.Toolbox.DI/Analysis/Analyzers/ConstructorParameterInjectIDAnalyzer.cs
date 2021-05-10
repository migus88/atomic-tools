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
    public class ConstructorParameterInjectIDAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticID = AnalyzerID.ConstructorParameterInjectID;
        private const string Title = "ID argument missing";
        private const string MessageFormat = "Inject attribute assigned to a constructor parameter must have an ID";
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
                var diagnostics = @class.Constructors
                    .SelectMany(constructor => constructor.Parameters)
                    .Where(parameter => parameter.HasAttribute(DiAttributes.InjectType))
                    .Select(parameter => parameter.GetAttribute(DiAttributes.InjectType))
                    .Where(attribute => !attribute.HasArgument(DiAttributes.Fields.ID))
                    .Select(attribute => Diagnostic.Create(Rule, attribute.Location))
                    .ToList();

                diagnostics.ForEach(context.ReportDiagnostic);
            }
        }
    }
}
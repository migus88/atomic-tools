using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Atomic.Generators.Tools.Enums;
using Atomic.Generators.Tools.Parsers;
using Atomic.Injector.Generators.Helpers.Identifiers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static Atomic.Injector.Generators.Helpers.Identifiers.InstallAttributeTypes;

namespace Atomic.Injector.Generators.Analyzers
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
                    .Where(parameter => parameter.HasAttribute(InjectAttributeType))
                    .Select(parameter => parameter.GetAttribute(InjectAttributeType))
                    .Where(attribute => !attribute.HasArgument(InstallAttributeArguments.ID))
                    .Select(attribute => Diagnostic.Create(Rule, attribute.Location))
                    .ToList();

                diagnostics.ForEach(context.ReportDiagnostic);
            }
        }
    }
}
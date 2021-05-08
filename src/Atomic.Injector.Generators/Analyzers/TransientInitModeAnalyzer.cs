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
    public class TransientInitModeAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticID = AnalyzerID.TransientInitMode;
        private const string Title = "InitMode for transient install is ignored";
        private const string MessageFormat = "InitMode for transient install is ignored";
        private const string Category = "Declaration";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticID, Title, MessageFormat,
            Category, DiagnosticSeverity.Warning, true);

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

            var classes = tree.GetClassesWithInterface(ContainerInterfaceType);

            foreach (var @class in classes)
            {
                var fields = @class.GetFieldsWithAttributes(TransientAttributeType);

                var diagnostics = fields
                    .SelectMany(field => field.GetAttributes(TransientAttributeType))
                    .Where(attribute => attribute.HasArgument(InstallAttributeArguments.InitMode))
                    .Select(attribute => attribute.GetArgument(InstallAttributeArguments.InitMode))
                    .Select(argument => Diagnostic.Create(Rule, argument.Location))
                    .ToList();

                diagnostics.ForEach(context.ReportDiagnostic);
            }
        }
    }
}
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
    public class InterfacesMustHaveImplementationAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticID = AnalyzerID.InterfacesMustHaveImplementation;
        private const string Title = "Interfaces must have implementation";
        private const string MessageFormat = "Interface installation missing 'BindTo' argument";
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

            if (classes.Count == 0)
            {
                return;
            }

            var parser = new Parser(context.SemanticModel.Compilation);

            foreach (var @class in classes)
            {
                var diagnostics = @class
                    .GetFieldsWithAttributes(DiAttributes.InstallTypes)
                    .Where(field => field.GetAttributes(DiAttributes.InstallTypes)
                        .Any(attribute => !attribute.HasArgument(DiAttributes.Fields.BindTo)))
                    .Where(field => IsInterface(field.TypeName, parser))
                    .Select(field => Diagnostic.Create(Rule, field.Location))
                    .ToList();

                diagnostics.ForEach(context.ReportDiagnostic);
            }
        }

        private bool IsInterface(string fieldTypeName, Parser parser)
        {
            return parser.Trees
                .SelectMany(tree => tree.Interfaces)
                .Any(i => i.FullInterfaceName == fieldTypeName);
        }
    }
}
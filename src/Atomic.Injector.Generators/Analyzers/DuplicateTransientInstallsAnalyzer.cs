using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Atomic.Generators.Tools.Parsers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static Atomic.Injector.Generators.Helpers.Identifiers.InstallAttributeTypes;

namespace Atomic.Injector.Generators.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DuplicateTransientInstallsAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticID = "ATOM01";
        private const string Title = "Multiple Transient installs for the same type";
        private const string MessageFormat = "Field can't be registered as Transient more than once without a scope";
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
            
            
            var classes = tree.GetClassesWithInterface(ContainerInterfaceType);

            foreach (var @class in classes)
            {
                var fields = @class.GetFieldsWithAttributes(TransientAttributeType);

                var diagnostics = fields
                    .GroupBy(f => f.TypeName)
                    .Select(g => g.ToList())
                    .Where(g => g.Count > 1)
                    .Select(g => g.First())
                    .Select(f => Diagnostic.Create(Rule, f.Location))
                    .ToList();
                
                diagnostics.ForEach(context.ReportDiagnostic);
            }
        }
    }
}
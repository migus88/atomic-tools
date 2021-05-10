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
    public class ContainerEmptyConstructorAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticID = AnalyzerID.ContainerEmptyConstructor;
        private const string Title = "Container has empty constructo";
        private const string MessageFormat = "Container can't have an empty constructor";
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
            if (context.SemanticModel.SyntaxTree.FilePath.Contains(".Generated.cs"))
            {
                return;
            }
            
            var tree = new Tree(context.SemanticModel);

            var classes = tree.GetClassesWithInterface(DiInterfaces.ContainerType);

            foreach (var @class in classes)
            {
                var diagnostics = @class.Constructors
                    .Where(c => c.Parameters == null || c.Parameters.Count == 0)
                    .Select(c => Diagnostic.Create(Rule, c.Location)).ToList();

                diagnostics.ForEach(context.ReportDiagnostic);
            }
        }
    }
}
using System.Collections.Generic;
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
    public class MultipleSingletonInstallationsAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticID = AnalyzerID.MultipleSingletonInstallations;
        private const string Title = "Only one singleton installation allowed";
        private const string MessageFormat = "Singleton can be registered only once";
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
            var diagnostics = new List<Diagnostic>();

            foreach (var @class in classes)
            {
                var singletonFields = @class.GetFieldsWithAttributes(DiAttributes.SingletonType);
                var nonSingletonFields = @class.GetFieldsWithAttributes(DiAttributes.ScopedType, DiAttributes.TransientType);

                foreach (var singletonField in singletonFields)
                {
                    var diags = nonSingletonFields
                        .Where(field => field.TypeName == singletonField.TypeName)
                        .Select(field => Diagnostic.Create(Rule, field.Location));
                    diagnostics.AddRange(diags);
                }
            }
            

            diagnostics.ForEach(context.ReportDiagnostic);
        }
    }
}
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
    public class DuplicateTransientIDDeclarationAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticID = AnalyzerID.DuplicateTransientIDDeclaration;
        private const string Title = "Multiple Transient installs for the same type with the same ID";
        private const string MessageFormat =
            "Multiple fields of the same type can't be registered as Transient more than once with the same ID";
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
                var diagnostics = new List<Diagnostic>();

                foreach (var field in fields)
                {
                    var attributes = field.GetAttributes(TransientAttributeType);
                    var existingIDs = new List<string>();
                    
                    var arguments = attributes
                        .Select(attribute => attribute.GetArgument(InstallAttributeArguments.ID))
                        .Where(argument => argument != null);
                    
                    foreach (var argument in arguments)
                    {
                        if (existingIDs.Contains(argument.Value))
                        {
                            diagnostics.Add(Diagnostic.Create(Rule, argument.Location));
                            continue;
                        }
                        
                        existingIDs.Add(argument.Value);
                    }
                }

                diagnostics.ForEach(context.ReportDiagnostic);
            }
        }
    }
}
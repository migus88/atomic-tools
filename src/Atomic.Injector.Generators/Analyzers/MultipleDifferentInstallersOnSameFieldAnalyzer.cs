using System;
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
    public class MultipleDifferentInstallersOnSameFieldAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticID = AnalyzerID.MultipleDifferentInstallersOnSameField;
        private const string Title = "Multiple different Install Attributes";
        private const string MessageFormat = "Field cannot have different install attributes";
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

            var searchedAttributes = new Type[]
            {
                ScopedAttributeType, 
                TransientAttributeType,
                SingletonAttributeType
            };

            foreach (var @class in classes)
            {
                var fields = @class.GetFieldsWithAttributes(searchedAttributes);

                var diagnostics = fields
                    .Where(field => field.Attributes.Count > 1)
                    .Where(field => field
                        .CountAttributes(searchedAttributes)
                        .Count(pair => pair.count > 0) > 1)
                    .Select(field => Diagnostic.Create(Rule, field.Location))
                    .ToList();

                diagnostics.ForEach(context.ReportDiagnostic);
            }
        }
    }
}
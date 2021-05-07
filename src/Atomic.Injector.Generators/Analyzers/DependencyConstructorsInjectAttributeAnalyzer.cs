using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Atomic.Generators.Tools.Helpers;
using Atomic.Generators.Tools.Parsers;
using Atomic.Injector.Generators.Helpers.Identifiers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static Atomic.Injector.Generators.Helpers.Identifiers.InstallAttributeTypes;

namespace Atomic.Injector.Generators.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DependencyConstructorsInjectAttributeAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticID = AnalyzerID.DependencyConstructorsInjectAttribute;
        private const string Title = "Inject attribute is missing";

        private const string MessageFormat =
            "Injectable constructor not found. Please add Inject attribute to one of the constructors.";

        private const string Category = "Declaration";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticID, Title, MessageFormat,
            Category, DiagnosticSeverity.Error, true);

        private static readonly Type[] InstallAttributes = new Type[]
        {
            ScopedAttributeType,
            TransientAttributeType,
            SingletonAttributeType
        };

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                                   GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSemanticModelAction(Analyze);
        }

        private void Analyze(SemanticModelAnalysisContext context)
        {
            var parser = new Parser(context.SemanticModel.Compilation);
            var dependencies = new List<string>();
            
            var diagnostics = new List<Diagnostic>();

            foreach (var tree in parser.Trees)
            {
                var classes = tree.GetClassesWithInterface(ContainerInterfaceType);

                if (classes.Any())
                {
                    var containerDependencies = GetDependenciesList(classes);
                    dependencies.AddRange(containerDependencies);
                }
            }

            if (dependencies.Count == 0)
            {
                return;
            }

            var trees = parser.Trees
                .Where(tree => tree.Classes.Any(@class => dependencies.Any(dependency =>
                    dependency == HelperMethods.GetFullClassName(tree.Namespace, @class.ClassName)))).ToList();

            foreach (var tree in trees)
            {
                var dependencyDiagnostics = GetDependencyDiagnostics(tree);
                diagnostics.AddRange(dependencyDiagnostics);
            }
            
            diagnostics.ForEach(context.ReportDiagnostic);
        }

        private List<Diagnostic> GetDependencyDiagnostics(Tree tree)
        {
            var diagnostics = new List<Diagnostic>();

            foreach (var @class in tree.Classes)
            {
                var fullClassName = HelperMethods.GetFullClassName(tree.Namespace, @class.ClassName);
                if (@class.Constructors.Count == 0
                    || @class.Constructors.Any(c => c.HasAttribute(InjectAttributeType)))
                {
                    continue;
                }

                diagnostics.Add(Diagnostic.Create(Rule, @class.Location));
            }

            return diagnostics;
        }

        private List<string> GetDependenciesList(List<Class> containers)
        {
            var explicitTypes = containers
                .SelectMany(container => container.GetFieldsWithAttributes(InstallAttributes))
                .SelectMany(field => field.GetAttributes(InstallAttributes))
                .Where(attribute => attribute.HasArgument(InstallAttributeArguments.BindTo))
                .Select(attribute => attribute.GetArgument(InstallAttributeArguments.BindTo))
                .Select(argument => argument.Value)
                .ToList();

            var implicitTypes = containers
                .SelectMany(container => container.GetFieldsWithAttributes(InstallAttributes))
                .Select(field => field.TypeName)
                .ToList();

            var dependencies = new List<string>();
            dependencies.AddRange(explicitTypes);
            dependencies.AddRange(implicitTypes);

            return dependencies;
        }
    }
}
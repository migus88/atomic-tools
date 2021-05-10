using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Atomic.Toolbox.Core.Helpers;
using Atomic.Toolbox.Core.Parsers;
using Atomic.Toolbox.DI.Analyzers;
using Atomic.Toolbox.DI.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Atomic.Toolbox.DI.Analysis.Analyzers
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
                var classes = tree.GetClassesWithInterface(DiInterfaces.ContainerType);

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
                    || @class.Constructors.Any(c => c.HasAttribute(DiAttributes.InjectType)))
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
                .SelectMany(container => container.GetFieldsWithAttributes(DiAttributes.InstallTypes))
                .SelectMany(field => field.GetAttributes(DiAttributes.InstallTypes))
                .Where(attribute => attribute.HasArgument(DiAttributes.Fields.BindTo))
                .Select(attribute => attribute.GetArgument(DiAttributes.Fields.BindTo))
                .Select(argument => argument.Value)
                .ToList();

            var implicitTypes = containers
                .SelectMany(container => container.GetFieldsWithAttributes(DiAttributes.InstallTypes))
                .Select(field => field.TypeName)
                .ToList();

            var dependencies = new List<string>();
            dependencies.AddRange(explicitTypes);
            dependencies.AddRange(implicitTypes);

            return dependencies;
        }
    }
}
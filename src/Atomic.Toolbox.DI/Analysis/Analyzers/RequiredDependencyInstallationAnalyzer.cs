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
    public class RequiredDependencyInstallationAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticID = AnalyzerID.RequiredDependencyInstallation;
        private const string Title = "Missing dependency installation";

        private const string MessageFormat = "Not all dependencies are installed";

        private const string Category = "Initialization";

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
            var tree = new Tree(context.SemanticModel);

            if (!tree.HasClassWithInterface(DiInterfaces.ContainerType))
            {
                return;
            }

            var parser = new Parser(context.SemanticModel.Compilation);

            var diagnostics = new List<Diagnostic>();
            var classes = tree.GetClassesWithInterface(DiInterfaces.ContainerType);

            var installedClasses = GetInstalledClasses(classes);

            foreach (var installedClass in installedClasses)
            {
                foreach (var parserTree in parser.Trees)
                {
                    var @class = parserTree.Classes.FirstOrDefault(c =>
                        HelperMethods.GetFullClassName(parserTree.Namespace, c.ClassName) == installedClass.typeName);

                    if (@class == null)
                        continue;

                    var installedTypes = installedClasses.Select(c => c.typeName).ToList();

                    var classDiagnostics =
                        GetInstalledClassesDiagnostics(@class, installedTypes, installedClass.location);
                    diagnostics.AddRange(classDiagnostics);
                }
            }

            diagnostics.ForEach(context.ReportDiagnostic);
        }

        private List<Diagnostic> GetInstalledClassesDiagnostics(Class @class, List<string> installedClasses,
            Location location)
        {
            var diagnostics = new List<Diagnostic>();

            if (@class.Constructors.Count == 0
                || @class.Constructors.All(c => !c.HasAttribute(DiAttributes.InjectType))
                || @class.Constructors.All(c => c.Parameters.Count == 0))
            {
                return new List<Diagnostic>();
            }

            var constructor = @class.GetConstructorsWithAttribute(DiAttributes.InjectType).First();

            foreach (var parameter in constructor.Parameters)
            {
                if (installedClasses.All(c => c != parameter.Type))
                {
                    diagnostics.Add(Diagnostic.Create(Rule, location));
                }
            }

            return diagnostics;
        }

        private List<(Location location, string typeName)> GetInstalledClasses(List<Class> containers)
        {
            var explicitTypes = containers
                .SelectMany(container => container.GetFieldsWithAttributes(DiAttributes.InstallTypes))
                .SelectMany(field => field.GetAttributes(DiAttributes.InstallTypes))
                .Where(attribute => attribute.HasArgument(DiAttributes.Fields.BindTo))
                .Select(attribute => attribute.GetArgument(DiAttributes.Fields.BindTo))
                .Select(argument => (argument.Location, argument.Value))
                .ToList();

            var implicitTypes = containers
                .SelectMany(container => container.GetFieldsWithAttributes(DiAttributes.InstallTypes))
                .Select(field => (field.Location, field.TypeName))
                .ToList();

            var dependencies = new List<(Location, string)>();
            dependencies.AddRange(explicitTypes);
            dependencies.AddRange(implicitTypes);

            return dependencies;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomic.Toolbox.Core.Exceptions;
using Atomic.Toolbox.Core.Helpers;
using Atomic.Toolbox.Core.Parsers;
using Atomic.Toolbox.DI.Core.Attributes;
using Atomic.Toolbox.DI.Core.Enums;
using Atomic.Toolbox.DI.Core.Interfaces;
using Atomic.Toolbox.DI.Analyzers;
using Atomic.Toolbox.DI.Models;
using Atomic.Toolbox.DI.Enums;
using Atomic.Toolbox.DI.Generation;
using Atomic.Toolbox.DI.Generation.Definitions;
using Atomic.Toolbox.DI.Generation.Models;
using Atomic.Toolbox.DI.Generation.Models.Fields;
using Atomic.Toolbox.DI.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Attribute = Atomic.Toolbox.Core.Parsers.Attribute;


namespace Atomic.Toolbox.DI
{
    public class DiContainerGenerationHandler : IGenerationHandler
    {
        private const string SourceNameTemplate = "{0}.{1}.Generated.cs";

        public bool HasSources => _containers is {Count: > 0};
        public List<SourceModel> Sources => _containers;

        private readonly Parser _parser;

        private List<SourceModel> _containers;

        public DiContainerGenerationHandler(Parser parser)
        {
            _parser = parser;
        }

        public void Generate()
        {
            if (HasSources)
            {
                return;
            }

            _containers = new List<SourceModel>();

            HandleTrees();
        }

        private void HandleTrees()
        {
            var trees = _parser.GetTreesWithInterface(DiInterfaces.ContainerType);

            foreach (var tree in trees)
            {
                HandleTree(tree);
            }
        }

        private void HandleTree(Tree tree)
        {
            var treeClasses = tree.GetClassesWithInterface(DiInterfaces.ContainerType);
            var treeContainers = GenerateContainerSources(treeClasses, tree.UsingString, tree.Namespace);

            if (treeContainers.Count > 0)
            {
                _containers.AddRange(treeContainers);
            }
        }

        private List<SourceModel> GenerateContainerSources(IEnumerable<Class> classes, string usingString,
            string namespaceString)
        {
            var sources = new List<SourceModel>();

            foreach (var @class in classes)
            {
                var container = GenerateContainerSource(usingString, namespaceString, @class);
                sources.Add(container);
            }

            return sources;
        }

        private SourceModel GenerateContainerSource(string usingString, string namespaceString, Class @class)
        {
            var fields = GetFields(@class);

            var container =
                new ContainerModel(usingString, namespaceString, @class.ClassName, fields.ToArray());

            return new SourceModel
            {
                ClassName = string.Format(SourceNameTemplate, namespaceString, @class.ClassName),
                Source = SourceText.From(container.ToString(), Encoding.UTF8)
            };
        }

        private List<IFieldModel> GetFields(Class @class)
        {
            var fields = @class.GetFieldsWithAttributes(DiAttributes.InstallTypes);

            return (
                from field in fields
                let installDefinitions = GetInstallDefinitions(field, field.TypeName, fields)
                select BaseFieldModel.GetFieldModel(installDefinitions)
            ).ToList();
        }

        private List<InstallDefinition> GetInstallDefinitions(Field field, string interfaceName, List<Field> allFields)
        {
            var attributes = field.GetAttributes(DiAttributes.InstallTypes);

            var definitions = new List<InstallDefinition>();

            foreach (var attribute in attributes)
            {
                var definition = GetInstallDefinition(field, interfaceName, attribute, allFields);

                if (definition != null)
                {
                    definitions.Add(definition);
                }
            }

            return definitions;
        }

        private InstallDefinition GetInstallDefinition(Field field, string interfaceName, Attribute attribute,
            List<Field> allFields)
        {
            var definition = new InstallDefinition
            {
                InterfaceName = interfaceName,
                IsLazy = false,
                BoundType = field.TypeName,
                Mode = attribute.GetInstallMode(),
                PrivateFieldName = field.Name
            };

            var lazyArgument = attribute.GetArgument(DiAttributes.Fields.InitMode);
            definition.IsLazy = lazyArgument != null && lazyArgument.Value == InitMode.Lazy.ToFullString();

            var bindArgument = attribute.GetArgument(DiAttributes.Fields.BindTo);
            definition.BoundType = bindArgument != null ? bindArgument.Value : field.TypeName;

            var idArgument = attribute.GetArgument(DiAttributes.Fields.ID);
            definition.ID = idArgument?.Value ?? ContainerModel.DefaultID;

            definition.Dependencies = GetDependencies(definition, allFields);

            return definition;
        }

        private List<DependencyDefinition> GetDependencies(InstallDefinition installDefinition, List<Field> allFields)
        {
            var dependencies = new List<DependencyDefinition>();

            var @class = GetClassOfType(installDefinition.BoundType);
            var constructor = GetInjectableConstructor(installDefinition.BoundType, @class);

            if (constructor == null)
            {
                dependencies.Add(new DependencyDefinition());
                return dependencies;
            }

            foreach (var parameter in constructor.Parameters)
            {
                var dependency = GetDependency(allFields, parameter);

                if (dependency != null)
                {
                    dependencies.Add(dependency);
                }
            }

            return dependencies;
        }

        private DependencyDefinition GetDependency(List<Field> allFields, Parameter parameter)
        {
            var dependency = new DependencyDefinition(parameter.Type);

            if (parameter.HasAttribute(DiAttributes.InjectType))
            {
                dependency.ID = GetDependencyID(parameter);
            }

            var dependencyField = FindDependencyField(dependency, allFields);

            if (dependencyField.HasAnyAttribute(DiAttributes.ScopedType, DiAttributes.TransientType))
            {
                dependency.Mode = InstallMode.Scoped | InstallMode.Transient;
            }

            dependency.PropertyName = dependencyField.Name.ToPascalCase();

            return dependency;
        }

        private string GetDependencyID(Parameter parameter)
        {
            var attribute = parameter.GetAttribute(DiAttributes.InjectType);

            return attribute.HasArgument(DiAttributes.Fields.ID)
                ? attribute.GetArgument(DiAttributes.Fields.ID).Value
                : string.Empty;
        }

        private Field FindDependencyField(DependencyDefinition dependency, List<Field> allFields)
        {
            return (
                from field in allFields
                where field.HasAnyAttribute(DiAttributes.InstallTypes) &&
                      field.TypeName == dependency.TypeName
                select field
            ).FirstOrDefault();
        }

        private Class GetClassOfType(string bindingType)
        {
            var @class = _parser.GetClass(bindingType);

            if (@class == null)
            {
                throw new Exception("Class not found");
            }

            return @class;
        }

        private Constructor GetInjectableConstructor(string bindingType, Class @class)
        {
            var constructors = @class.GetConstructorsWithAttribute(DiAttributes.InjectType);

            if (constructors.Count > 1)
            {
                throw new GeneratorException(
                    $"'{bindingType}' should have only one constructor with '{nameof(InjectAttribute)}'");
            }

            var constructor = constructors.Count == 0
                ? @class.GetDefaultConstructor()
                : constructors[0];

            if (constructor == null && @class.HasAnyConstructor())
            {
                throw new GeneratorException(
                    $"Can't find injection constructor for '{bindingType}'. Either provide an empty constructor, or annotate one constructor with '{nameof(InjectAttribute)}'");
            }

            return constructor;
        }
    }
}
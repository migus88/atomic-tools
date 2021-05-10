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
using Atomic.Toolbox.DI.Generation.Definitions;
using Atomic.Toolbox.DI.Generation.Models;
using Atomic.Toolbox.DI.Generation.Models.Fields;
using Atomic.Toolbox.DI.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Attribute = Atomic.Toolbox.Core.Parsers.Attribute;


namespace Atomic.Toolbox.DI
{
    public class DiContainerHandler
    {
        private readonly Parser _parser;

        private List<(string ClassName, SourceText Source)> _containers;

        public DiContainerHandler(Parser parser)
        {
            _parser = parser;
        }


        public List<(string ClassName, SourceText Source)> GetContainers()
        {
            if (_containers != null && _containers.Any())
            {
                return _containers;
            }

            var trees = _parser.GetTreesWithInterface(DiInterfaces.ContainerType);
            _containers = new List<(string ClassName, SourceText Source)>();

            foreach (var tree in trees)
            {
                var containerClasses = tree.GetClassesWithInterface(DiInterfaces.ContainerType);
                var newContainers =
                    GenerateContainers(containerClasses, tree.UsingString, tree.Namespace);
                _containers.AddRange(newContainers);
            }

            return _containers;
        }

        private List<(string ClassName, SourceText Source)> GenerateContainers(List<Class> containerClasses,
            string usingString, string namespaceString)
        {
            var containers = new List<(string ClassName, SourceText Source)>();
            foreach (var @class in containerClasses)
            {
                var container = GenerateContainer(usingString, namespaceString, @class);
                containers.Add(container);
            }

            return containers;
        }

        private (string ClassName, SourceText Source) GenerateContainer(string usingString, string namespaceString,
            Class @class)
        {
            var className = @class.ClassName;

            var fieldModels = GetFieldModels(@class);

            var containerModel =
                new ContainerModel(usingString, namespaceString, className, fieldModels.ToArray());

            var generatedClass = containerModel.ToString();
            return ($"{namespaceString}.{className}.Generated.cs",
                SourceText.From(generatedClass, Encoding.UTF8));
        }

        private List<IFieldModel> GetFieldModels(Class @class)
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
            var attributes =
                field.GetAttributes(DiAttributes.InstallTypes);

            var definitions = new List<InstallDefinition>();

            foreach (var attribute in attributes)
            {
                var installDefinition = new InstallDefinition
                {
                    InterfaceName = interfaceName,
                    IsLazy = false,
                    BoundType = field.TypeName,
                    Mode = GetInstallMode(attribute),
                    PrivateFieldName = field.Name
                };

                var lazyArgument = attribute.GetArgument(DiAttributes.Fields.InitMode);
                installDefinition.IsLazy = lazyArgument != null && lazyArgument.Value == InitMode.Lazy.ToFullString();

                var bindArgument = attribute.GetArgument(DiAttributes.Fields.BindTo);
                installDefinition.BoundType = bindArgument != null ? bindArgument.Value : field.TypeName;

                var idArgument = attribute.GetArgument(DiAttributes.Fields.ID);
                installDefinition.ID = idArgument?.Value ?? ContainerModel.DefaultID;

                definitions.Add(installDefinition);
            }

            UpdateDependencyStrings(ref definitions, allFields);

            return definitions;
        }

        private InstallMode GetInstallMode(Attribute attribute)
        {
            if (attribute.TypeName == DiAttributes.SingletonType.FullName)
            {
                return InstallMode.Singleton;
            }

            if (attribute.TypeName == DiAttributes.ScopedType.FullName)
            {
                return InstallMode.Scoped;
            }

            return InstallMode.Transient;
        }

        private void UpdateDependencyStrings(ref List<InstallDefinition> installDefinitions, List<Field> allFields)
        {
            foreach (var installDefinition in installDefinitions)
            {
                UpdateDependencies(installDefinition, allFields);
            }
        }

        private void UpdateDependencies(InstallDefinition installDefinition, List<Field> allFields)
        {
            var @class = GetClassOfType(installDefinition.BoundType);
            var constructor = GetInjectableConstructor(installDefinition.BoundType, @class);

            if (constructor == null)
            {
                installDefinition.Dependencies.Add(new DependencyDefinition());
                return;
            }

            foreach (var parameter in constructor.Parameters)
            {
                var dependency = new DependencyDefinition(parameter.Type);

                if (parameter.HasAttribute(DiAttributes.InjectType))
                {
                    var attribute = parameter.GetAttribute(DiAttributes.InjectType);

                    if (attribute.HasArgument(DiAttributes.Fields.ID))
                    {
                        dependency.ID = attribute.GetArgument(DiAttributes.Fields.ID).Value;
                    }
                }

                var dependencyField = FindDependencyField(dependency, allFields);

                if (dependencyField.HasAnyAttribute(DiAttributes.ScopedType, DiAttributes.TransientType))
                {
                    dependency.Mode = InstallMode.Scoped | InstallMode.Transient;
                }
                
                dependency.PropertyName = dependencyField.Name.ToPascalCase();

                installDefinition.Dependencies.Add(dependency);
            }
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
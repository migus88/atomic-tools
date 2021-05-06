using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomic.Generators.Tools.Exceptions;
using Atomic.Generators.Tools.Helpers;
using Atomic.Generators.Tools.Parsers;
using Atomic.Injector.Core.Attributes;
using Atomic.Injector.Core.Enums;
using Atomic.Injector.Core.Interfaces;
using Atomic.Injector.Generators.Analyzers;
using Atomic.Injector.Generators.Definitions;
using Atomic.Injector.Generators.Enums;
using Atomic.Injector.Generators.Exceptions;
using Atomic.Injector.Generators.Exceptions.Analyzers;
using Atomic.Injector.Generators.Helpers;
using Atomic.Injector.Generators.Helpers.Identifiers;
using Atomic.Injector.Generators.Interfaces;
using Atomic.Injector.Generators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Attribute = Atomic.Generators.Tools.Parsers.Attribute;
using static Atomic.Injector.Generators.Helpers.Identifiers.InstallAttributeTypes;

namespace Atomic.Injector.Generators
{
    public class ContainersGenerator
    {
        private readonly GeneratorExecutionContext _context;
        private readonly Parser _parser;

        private List<(string ClassName, SourceText Source)> _containers;

        public ContainersGenerator(GeneratorExecutionContext context, Parser parser)
        {
            _context = context;
            _parser = parser;
        }


        public List<(string ClassName, SourceText Source)> GetContainers()
        {
            if (_containers != null && _containers.Any())
            {
                return _containers;
            }

            var trees = _parser.GetTreesWithInterface(ContainerInterfaceType);
            _containers = new List<(string ClassName, SourceText Source)>();

            foreach (var tree in trees)
            {
                var containerClasses = tree.GetClassesWithInterface(ContainerInterfaceType);
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
            var fields = @class.GetFieldsWithAttributes(SingletonAttributeType, ScopedAttributeType,
                TransientAttributeType);

            return (
                from field in fields
                let installDefinitions = GetInstallDefinitions(field, field.TypeName, fields)
                select BaseFieldModel.GetFieldModel(installDefinitions)
            ).ToList();
        }

        private List<InstallDefinition> GetInstallDefinitions(Field field, string interfaceName, List<Field> allFields)
        {
            var attributes =
                field.GetAttributes(SingletonAttributeType, ScopedAttributeType, TransientAttributeType);

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

                var lazyArgument = attribute.GetArgument(InstallAttributeArguments.InitMode);
                installDefinition.IsLazy = lazyArgument != null && lazyArgument.Value == InitMode.Lazy.ToFullString();

                var bindArgument = attribute.GetArgument(InstallAttributeArguments.BindTo);
                installDefinition.BoundType = bindArgument != null ? bindArgument.Value : field.TypeName;

                var idArgument = attribute.GetArgument(InstallAttributeArguments.ID);
                installDefinition.ID = idArgument?.Value ?? string.Empty;

                definitions.Add(installDefinition);
            }

            UpdateDependencyStrings(ref definitions, allFields);

            return definitions;
        }

        private InstallMode GetInstallMode(Attribute attribute)
        {
            if (attribute.TypeName == SingletonAttributeType.FullName)
            {
                return InstallMode.Singleton;
            }

            if (attribute.TypeName == ScopedAttributeType.FullName)
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

                if (parameter.HasAttribute(InjectAttributeType))
                {
                    dependency.Mode = InstallMode.Scoped;
                    
                    var attribute = parameter.GetAttribute(InjectAttributeType);

                    if (attribute.HasArgument(InstallAttributeArguments.ID))
                    {
                        dependency.ID = attribute.GetArgument(InstallAttributeArguments.ID).Value;
                    }
                }
                else
                {
                    dependency.Mode = InstallMode.Singleton | InstallMode.Transient;
                }

                var dependencyField = FindDependencyField(dependency, allFields);
                dependency.PropertyName = dependencyField.Name.ToPascalCase();

                installDefinition.Dependencies.Add(dependency);
            }
        }

        private Field FindDependencyField(DependencyDefinition dependency, List<Field> allFields)
        {
            if (dependency.Mode.HasFlag(InstallMode.Scoped))
            {
                return (
                    from field in allFields
                    where field.HasAttribute(ScopedAttributeType) && field.TypeName == dependency.TypeName
                    let attributes = field.GetAttributes(ScopedAttributeType)
                    from attribute in attributes
                    where attribute.HasArgument(InstallAttributeArguments.ID)
                    let argument = attribute.GetArgument(InstallAttributeArguments.ID)
                    where argument.Value == dependency.ID
                    select field
                ).FirstOrDefault();
            }

            return (
                from field in allFields
                where field.HasAnyAttribute(SingletonAttributeType, TransientAttributeType) &&
                      field.TypeName == dependency.TypeName
                select field
            ).FirstOrDefault();
        }

        private Class GetClassOfType(string bindingType)
        {
            var @class = _parser.GetClass(bindingType);

            if (@class == null)
            {
                throw new ClassNotFoundException(bindingType);
            }

            return @class;
        }

        private Constructor GetInjectableConstructor(string bindingType, Class @class)
        {
            var constructors = @class.GetConstructorsWithAttribute(InjectAttributeType);

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
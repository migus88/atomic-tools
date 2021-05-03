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
using Atomic.Injector.Generators.Enums;
using Atomic.Injector.Generators.Exceptions;
using Atomic.Injector.Generators.Exceptions.Analyzers;
using Atomic.Injector.Generators.Helpers;
using Atomic.Injector.Generators.Helpers.Identifiers;
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
                var analyzer = new InstallationAnalyzer(@class);
                analyzer.AnalyzeAll();
                
                var container = GenerateContainer(usingString, namespaceString, @class);
                containers.Add(container);
            }

            return containers;
        }

        private (string ClassName, SourceText Source) GenerateContainer(string usingString, string namespaceString,
            Class @class)
        {
            var className = @class.ClassName;

            var propertyModels = GetPropertyModels(@class);

            var containerModel =
                new ContainerModel(usingString, namespaceString, className, propertyModels.ToArray());

            var generatedClass = containerModel.ToString();
            return ($"Generated_{namespaceString.Replace(".", "_")}_{className}.cs",
                SourceText.From(generatedClass, Encoding.UTF8));
        }

        private List<PropertyModel> GetPropertyModels(Class @class)
        {
            var fields = @class.GetFieldsWithAttributes(SingletonAttributeType, ScopedAttributeType,
                TransientAttributeType);

            return (
                from field in fields
                let installModel = GetInstallModel(field)
                let dependencies = GetDependencyStrings(installModel.BoundType, fields)
                select new PropertyModel(field.TypeName, installModel.BoundType, field.Name, installModel,
                    dependencies.ToArray())
            ).ToList();
        }

        private InstallModel GetInstallModel(Field field)
        {
            var attributes =
                field.GetAttributes(SingletonAttributeType, ScopedAttributeType, TransientAttributeType);

            if (attributes.Count > 1)
            {
                throw new MultipleInstallAttributesException(field.Name);
            }

            var attribute = attributes.First();

            var installModel = new InstallModel
            {
                IsLazy = false, 
                BoundType = field.TypeName, 
                Mode = GetInstallMode(attribute),
            };


            var lazyArgument = attribute.GetArgument(InstallAttributeArguments.InitMode);
            installModel.IsLazy = lazyArgument != null && lazyArgument.Value == InitMode.Lazy.ToString();

            var bindArgument = attribute.GetArgument(InstallAttributeArguments.BindTo);
            installModel.BoundType = bindArgument != null ? bindArgument.Value : field.TypeName;

            //TODO: Implement
            var idArgument = attribute.GetArgument(InstallAttributeArguments.ID);

            return installModel;
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

        private List<string> GetDependencyStrings(string bindingType, List<Field> fields)
        {
            var @class = GetClassOfType(bindingType);

            var constructor = GetInjectableConstructor(bindingType, @class);

            return constructor == null
                ? new List<string>()
                : constructor
                    .Parameters
                    .Select(constructorParameter => GetDependencyField(bindingType, fields, constructorParameter))
                    .Select(dependencyField => dependencyField.Name.ToPascalCase())
                    .ToList();
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

        private static Field GetDependencyField(string bindingType, List<Field> fields, Parameter constructorParameter)
        {
            //TODO: Handle inline InjectAttribute and remove FirstOrDefault
            var dependencyField = fields.GetFieldsOfType(constructorParameter.Type).FirstOrDefault();

            if (dependencyField == null)
            {
                throw new GeneratorException(
                    $"Can't find dependency for '{bindingType}' of type: '{constructorParameter.Type}'");
            }

            return dependencyField;
        }
    }
}
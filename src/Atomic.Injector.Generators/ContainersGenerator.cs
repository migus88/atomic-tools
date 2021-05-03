using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomic.Generators.Tools.Exceptions;
using Atomic.Generators.Tools.Helpers;
using Atomic.Generators.Tools.Parsers;
using Atomic.Injector.Core.Attributes;
using Atomic.Injector.Core.Interfaces;
using Atomic.Injector.Generators.Exceptions;
using Atomic.Injector.Generators.Helpers;
using Atomic.Injector.Generators.Models;
using Atomic.Injector.Generators.Templates.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Atomic.Injector.Generators
{
    public class ContainersGenerator
    {
        private readonly Type _containerInterfaceType = typeof(IDiContainer);
        private readonly Type _singletonAttributeType = typeof(InstallSingletonAttribute);
        private readonly Type _scopedAttributeType = typeof(InstallScopedAttribute);
        private readonly Type _injectAttributeType = typeof(InjectAttribute);

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
            
            var trees = _parser.GetTreesWithInterface(_containerInterfaceType);
            _containers = new List<(string ClassName, SourceText Source)>();

            foreach (var tree in trees)
            {
                var containerClasses = tree.GetClassesWithInterface(_containerInterfaceType);
                var newContainers =
                    GenerateContainers(containerClasses, tree.UsingString, tree.Namespace);
                _containers.AddRange(newContainers);
            }

            return _containers;
        }

        private List<(string ClassName, SourceText Source)> GenerateContainers(List<Class> containerClasses, string usingString, string namespaceString)
        {
            var containers = new List<(string ClassName, SourceText Source)>();
            foreach (var classParser in containerClasses)
            {
                var container = GenerateContainer(usingString, namespaceString, classParser);
                containers.Add(container);
            }

            return containers;
        }

        private (string ClassName, SourceText Source) GenerateContainer(string usingString, string namespaceString, Class @class)
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
            var fields = @class.GetFieldsWithAttributes(_singletonAttributeType, _scopedAttributeType);

            return (
                from field in fields
                let installModel = GetInstallModel(field)
                let dependencies = GetDependencyStrings(installModel.BindingType, fields)
                select new PropertyModel(field.TypeName, installModel.BindingType, field.Name, installModel.IsLazy,
                    dependencies.ToArray())
            ).ToList();
        }

        private InstallModel GetInstallModel(Field field)
        {
            var attributes = field.GetAttributes(_singletonAttributeType, _scopedAttributeType);

            //TODO: Handle error: Only one install attribute can be assigned to a field

            var installModel = new InstallModel
            {
                IsLazy = false,
                BindingType = field.TypeName
            };

            foreach (var attribute in attributes)
            {
                //TODO: Replace strings with debuggable values
                var lazyArgument = attribute.GetArgument("InitMode");
                installModel.IsLazy = lazyArgument != null && lazyArgument.Value == "InitMode.Lazy";

                var bindArgument = attribute.GetArgument("BindTo");
                installModel.BindingType = bindArgument != null ? bindArgument.Value : field.TypeName;

                //TODO: Implement
                var idArgument = attribute.GetArgument("ID");
            }

            return installModel;
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
            var constructors = @class.GetConstructorsWithAttribute(_injectAttributeType);

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
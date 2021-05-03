using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Atomic.Generators.Tools.Exceptions;
using Atomic.Injector.Core.Attributes;
using Atomic.Injector.Core.Interfaces;
using Atomic.Injector.Generators.Helpers;
using Atomic.Injector.Generators.Templates.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Atomic.Generators.Tools.Helpers;
using Atomic.Generators.Tools.Parsers;
using Atomic.Injector.Generators.Exceptions;
using Atomic.Injector.Generators.Models;
using Attribute = Atomic.Generators.Tools.Parsers.Attribute;

namespace Atomic.Injector.Generators
{
    [Generator]
    public class DiContainerGenerator : ISourceGenerator
    {
        private readonly Type _containerInterfaceType = typeof(IDiContainer);
        private readonly Type _singletonAttributeType = typeof(InstallSingletonAttribute);
        private readonly Type _scopedAttributeType = typeof(InstallScopedAttribute);
        private readonly Type _injectAttributeType = typeof(InjectAttribute);


        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG

            /*if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }*/
#endif
        }

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                HandleTrees(context);
            }
            catch (GeneratorException ex)
            {
                context.ShowError(ex);
            }
            catch (Exception ex)
            {
                context.ShowError(nameof(DiContainerGenerator), ex);
            }
        }

        private void HandleTrees(GeneratorExecutionContext context)
        {
            var parser = new Parser(context);
            
            var trees = parser.GetTreesWithInterface(_containerInterfaceType);

            foreach (var tree in trees)
            {
                HandleTree(context, tree, parser);
            }
        }

        private void HandleTree(GeneratorExecutionContext context, Tree tree, Parser parser)
        {
            var usingString = tree.UsingString;
            var namespaceString = tree.Namespace;

            var containerClasses = tree.GetClassesWithInterface(_containerInterfaceType);

            GenerateContainers(context, containerClasses, parser, usingString, namespaceString);
        }

        private void GenerateContainers(GeneratorExecutionContext context, List<Class> containerClasses, Parser parser,
            string usingString, string namespaceString)
        {
            foreach (var classParser in containerClasses)
            {
                GenerateContainer(context, parser, usingString, namespaceString, classParser);
            }
        }

        private void GenerateContainer(GeneratorExecutionContext context, Parser parser, string usingString,
            string namespaceString, Class classParser)
        {
            var className = classParser.ClassName;

            var propertyModels = GetPropertyModels(classParser, parser);

            var containerModel =
                new ContainerModel(usingString, namespaceString, className, propertyModels.ToArray());

            var generatedClass = containerModel.ToString();
            context.AddSource($"Generated_{className}.cs", SourceText.From(generatedClass, Encoding.UTF8));
        }

        private List<PropertyModel> GetPropertyModels(Class classParser, Parser parser)
        {
            var fields = classParser.GetFieldsWithAttributes(_singletonAttributeType, _scopedAttributeType);

            return (
                from field in fields
                let installModel = GetInstallModel(field)
                let dependencies = GetDependencyStrings(parser, installModel.BindingType, fields)
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

        private List<string> GetDependencyStrings(Parser parser, string bindingType, List<Field> fields)
        {
            var @class = GetClassOfType(parser, bindingType);

            var constructor = GetInjectableConstructor(bindingType, @class);

            return constructor == null
                ? new List<string>()
                : constructor
                    .Parameters
                    .Select(constructorParameter => GetDependencyField(bindingType, fields, constructorParameter))
                    .Select(dependencyField => dependencyField.Name.ToPascalCase())
                    .ToList();
        }

        private static Class GetClassOfType(Parser parser, string bindingType)
        {
            var @class = parser.GetClass(bindingType);

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
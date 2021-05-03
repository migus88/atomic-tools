using System;
using Atomic.Injector.Generators.Enums;
using Atomic.Injector.Generators.Helpers;
using Atomic.Injector.Generators.Helpers.Templates;

namespace Atomic.Injector.Generators.Models
{
    public class PropertyModel
    {
        public string InterfaceName { get; }
        public string ClassName { get; }
        public InstallModel InstallModel { get; }
        public string PrivateFieldName { get; }
        public string PropertyName => PrivateFieldName.ToPascalCase();
        public string[] Dependencies { get; }

        private static readonly string _propertyTemplate;
        private static readonly string _propertyInitializationTemplate;
        private static readonly string _constructorInitializationTemplate;
        private static readonly string _transientInitializationTemplate;
        private static readonly string _transientPropertyTemplate;

        static PropertyModel()
        {
            _propertyTemplate = ResourcesHelpers.GetTextResource(TemplatePaths.Property);
            _propertyInitializationTemplate = ResourcesHelpers.GetTextResource(TemplatePaths.PropertyInitialization);
            _constructorInitializationTemplate = ResourcesHelpers.GetTextResource(TemplatePaths.ConstructorInitialization);
            _transientInitializationTemplate = ResourcesHelpers.GetTextResource(TemplatePaths.TransientInitialization);
            _transientPropertyTemplate = ResourcesHelpers.GetTextResource(TemplatePaths.TransientProperty);
        }

        public PropertyModel(string interfaceName, string className, string privateFieldName, InstallModel installModel, string[] dependencies)
        {
            InterfaceName = interfaceName;
            ClassName = className;
            PrivateFieldName = privateFieldName;
            InstallModel = installModel;
            Dependencies = dependencies;
        }

        public string GetPropertyString()
        {
            return InstallModel.Mode switch
            {
                InstallMode.Singleton => GetPropertyString(_propertyTemplate),
                InstallMode.Scoped => GetPropertyString(_transientPropertyTemplate), //TODO: Implement
                InstallMode.Transient => GetPropertyString(_transientPropertyTemplate),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private string GetPropertyString(string template)
        {
            return template
                .Replace(Placeholders.ClassName, InterfaceName)
                .Replace(Placeholders.PropertyName, PropertyName)
                .Replace(Placeholders.PrivateFieldName, PrivateFieldName)
                .Replace(Placeholders.Initialization, GetPropertyInitialization());
        }

        private string GetDependenciesString() => string.Join(", ", Dependencies);

        private string GetPropertyInitialization()
        {
            return InstallModel.Mode switch
            {
                InstallMode.Singleton => GetPropertyInitializationString(),
                InstallMode.Scoped => GetScopedPropertyInitializationString(),
                InstallMode.Transient => GetTransientPropertyInitializationString(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public string GetConstructorInitializationString() =>
            GetInitializationString(_constructorInitializationTemplate);

        private string GetPropertyInitializationString() => 
            GetInitializationString(_propertyInitializationTemplate);

        private string GetTransientPropertyInitializationString() =>
            GetInitializationString(_transientInitializationTemplate);

        //TODO: Implement scoped
        private string GetScopedPropertyInitializationString() =>
            GetTransientPropertyInitializationString();

        private string GetInitializationString(string template)
        {
            return template
                .Replace(Placeholders.PrivateFieldName, PrivateFieldName)
                .Replace(Placeholders.ClassName, ClassName)
                .Replace(Placeholders.Dependencies, GetDependenciesString());
        }
    }
}
using System;
using Atomic.Injector.Generators.Definitions;
using Atomic.Injector.Generators.Enums;
using Atomic.Injector.Generators.Helpers;
using Atomic.Injector.Generators.Helpers.Templates;
using Atomic.Injector.Generators.Interfaces;

namespace Atomic.Injector.Generators.Models
{
    public class SingletonFieldModel : BaseFieldModel
    {
        private static readonly string _propertyTemplate;
        private static readonly string _propertyInitializationTemplate;
        private static readonly string _constructorInitializationTemplate;

        static SingletonFieldModel()
        {
            _propertyTemplate = ResourcesHelpers.GetTextResource(TemplatePaths.Property);
            _propertyInitializationTemplate = ResourcesHelpers.GetTextResource(TemplatePaths.PropertyInitialization);
            _constructorInitializationTemplate = ResourcesHelpers.GetTextResource(TemplatePaths.ConstructorInitialization);
        }
        
        public SingletonFieldModel(string interfaceName, string className, string privateFieldName,
            InstallDefinition installDefinition, string[] dependencies) : base(interfaceName, className,
            privateFieldName, installDefinition, dependencies)
        {
        }

        public override string GetPropertyString() => 
            GetPropertyString(_propertyTemplate);

        public override string GetConstructorString() => 
            _installDefinition.IsLazy ? string.Empty : GetInitializationString(_constructorInitializationTemplate);

        private string GetPropertyString(string template)
        {
            return template
                .Replace(Placeholders.ClassName, _interfaceName)
                .Replace(Placeholders.PropertyName, PropertyName)
                .Replace(Placeholders.PrivateFieldName, _privateFieldName)
                .Replace(Placeholders.Initialization, GetPropertyInitializationString());
        }

        private string GetPropertyInitializationString() => 
            GetInitializationString(_propertyInitializationTemplate);
        
        private string GetInitializationString(string template)
        {
            return template
                .Replace(Placeholders.PrivateFieldName, _privateFieldName)
                .Replace(Placeholders.ClassName, _className)
                .Replace(Placeholders.Dependencies, GetDependenciesString());
        }
        
        private string GetDependenciesString() => string.Join(", ", _dependencies);
    }
}
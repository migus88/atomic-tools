using System;
using Atomic.Injector.Generators.Definitions;
using Atomic.Injector.Generators.Enums;
using Atomic.Injector.Generators.Helpers;
using Atomic.Injector.Generators.Helpers.Templates;
using Atomic.Injector.Generators.Interfaces;

namespace Atomic.Injector.Generators.Models
{
    public class TransientFieldModel : BaseFieldModel
    {
        private static readonly string _propertyTemplate;
        private static readonly string _propertyInitializationTemplate;

        static TransientFieldModel()
        {
            _propertyTemplate = ResourcesHelpers.GetTextResource(TemplatePaths.TransientProperty);
            _propertyInitializationTemplate = ResourcesHelpers.GetTextResource(TemplatePaths.TransientInitialization);
        }
        
        public TransientFieldModel(string interfaceName, string className, string privateFieldName,
            InstallDefinition installDefinition, string[] dependencies) : base(interfaceName, className,
            privateFieldName, installDefinition, dependencies)
        {
        }

        public override string GetPropertyString() => GetPropertyString(_propertyTemplate);

        private string GetPropertyString(string template)
        {
            return template
                .Replace(Placeholders.ClassName, _interfaceName)
                .Replace(Placeholders.PropertyName, PropertyName)
                .Replace(Placeholders.PrivateFieldName, _privateFieldName)
                .Replace(Placeholders.Initialization, GetInitializationString());
        }
        
        private string GetInitializationString()
        {
            return _propertyInitializationTemplate
                .Replace(Placeholders.PrivateFieldName, _privateFieldName)
                .Replace(Placeholders.ClassName, _className)
                .Replace(Placeholders.Dependencies, GetDependenciesString());
        }
        
        private string GetDependenciesString() => string.Join(", ", _dependencies);
    }
}
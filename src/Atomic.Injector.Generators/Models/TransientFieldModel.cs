using System;
using System.Collections.Generic;
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
        
        public TransientFieldModel(List<InstallDefinition> installDefinitions) : base(installDefinitions)
        {
        }

        public override string GetPropertyString() => GetPropertyString(_propertyTemplate);

        private string GetPropertyString(string template)
        {
            return template
                .Replace(Placeholders.ClassName, _firstDefinition.InterfaceName)
                .Replace(Placeholders.PropertyName, _firstDefinition.PropertyName)
                .Replace(Placeholders.PrivateFieldName, _firstDefinition.PrivateFieldName)
                .Replace(Placeholders.Initialization, GetInitializationString());
        }
        
        private string GetInitializationString()
        {
            return _propertyInitializationTemplate
                .Replace(Placeholders.PrivateFieldName, _firstDefinition.PrivateFieldName)
                .Replace(Placeholders.ClassName, _firstDefinition.BoundType)
                .Replace(Placeholders.Dependencies, GetDependenciesString(_firstDefinition.Dependencies));
        }
    }
}
using System;
using System.Collections.Generic;
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
        
        public SingletonFieldModel(List<InstallDefinition> installDefinitions) : base(installDefinitions)
        {
        }

        public override string GetPropertyString() => 
            GetPropertyString(_propertyTemplate);

        public override string GetConstructorString() => 
            _firstDefinition.IsLazy ? string.Empty : GetInitializationString(_constructorInitializationTemplate, true, true);

        private string GetPropertyString(string template)
        {
            return template
                .Replace(Placeholders.ClassName, _firstDefinition.InterfaceName)
                .Replace(Placeholders.PropertyName, _firstDefinition.PropertyName)
                .Replace(Placeholders.PrivateFieldName, _firstDefinition.PrivateFieldName)
                .Replace(Placeholders.Initialization, GetPropertyInitializationString());
        }

        private string GetPropertyInitializationString() => 
            GetInitializationString(_propertyInitializationTemplate);
        
        private string GetInitializationString(string template, bool includeTabulation = false, bool includeLineBreak = false)
        {
            var generatedCode = template
                .Replace(Placeholders.PrivateFieldName, _firstDefinition.PrivateFieldName)
                .Replace(Placeholders.ClassName, _firstDefinition.BoundType)
                .Replace(Placeholders.Dependencies, GetDependenciesString(_firstDefinition.Dependencies));

            return $"{(includeTabulation ? GetTabSymbols(3) : string.Empty )}{generatedCode}{(includeLineBreak ? LineBreakSymbol : string.Empty)}";
        }
    }
}
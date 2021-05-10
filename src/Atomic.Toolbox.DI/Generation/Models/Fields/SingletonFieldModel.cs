using System.Collections.Generic;
using Atomic.Toolbox.DI.Helpers;
using Atomic.Toolbox.DI.Generation.Definitions;
using Atomic.Toolbox.DI.Helpers.Templates;

namespace Atomic.Toolbox.DI.Generation.Models.Fields
{
    public class SingletonFieldModel : BaseFieldModel
    {
        private static readonly string _propertyTemplate;
        private static readonly string _propertyInitializationTemplate;
        private static readonly string _constructorInitializationTemplate;

        static SingletonFieldModel()
        {
            _propertyTemplate = ResourceLoader.GetTextResource(TemplatePaths.Property);
            _propertyInitializationTemplate = ResourceLoader.GetTextResource(TemplatePaths.PropertyInitialization);
            _constructorInitializationTemplate = ResourceLoader.GetTextResource(TemplatePaths.ConstructorInitialization);
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
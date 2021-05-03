using Atomic.Injector.Generators.Definitions;
using Atomic.Injector.Generators.Helpers;
using Atomic.Injector.Generators.Helpers.Templates;

namespace Atomic.Injector.Generators.Models
{
    //TODO: Handle scope destruction
    public class ScopedFieldModel : BaseFieldModel
    {
        private static readonly string _dictionaryTemplate;
        private static readonly string _dictionaryInitializationTemplate;
        private static readonly string _getterTemplate;
        private static readonly string _initializationTemplate;

        static ScopedFieldModel()
        {
            _dictionaryTemplate = ResourcesHelpers.GetTextResource(TemplatePaths.Scope.Dictionary);
            _dictionaryInitializationTemplate =
                ResourcesHelpers.GetTextResource(TemplatePaths.Scope.DictionaryInitialization);
            _getterTemplate = ResourcesHelpers.GetTextResource(TemplatePaths.Scope.Getter);
            _initializationTemplate = ResourcesHelpers.GetTextResource(TemplatePaths.Scope.Initialization);
        }

        public ScopedFieldModel(string interfaceName, string className, string privateFieldName,
            InstallDefinition installDefinition, string[] dependencies) : base(interfaceName, className,
            privateFieldName, installDefinition, dependencies)
        {
        }

        public override string GetPrivateFieldString()
        {
            return _dictionaryTemplate
                .Replace(Placeholders.ClassName, _className)
                .Replace(Placeholders.PrivateFieldName, _privateFieldName)
                .Replace(Placeholders.Initialization, GetDictionaryInitializationString());
        }

        public override string GetMethodString()
        {
            return _getterTemplate
                .Replace(Placeholders.ClassName, _className)
                .Replace(Placeholders.PropertyName, PropertyName)
                .Replace(Placeholders.PrivateFieldName, _privateFieldName)
                .Replace(Placeholders.Initialization, GetClassInitializationString());
        }

        private string GetClassInitializationString()
        {
            return _initializationTemplate
                .Replace(Placeholders.ClassName, _className)
                .Replace(Placeholders.Dependencies, GetDependenciesString());
        }

        private string GetDictionaryInitializationString()
        {
            //TODO: Handle multiple IDs
            return _dictionaryInitializationTemplate
                .Replace(Placeholders.Scope, _installDefinition.ID)
                .Replace(Placeholders.Initialization, GetDictionaryFieldInitializationString());
        }

        private string GetDictionaryFieldInitializationString()
        {
            return _installDefinition.IsLazy
                ? "null"
                : GetClassInitializationString();
        }
    }
}
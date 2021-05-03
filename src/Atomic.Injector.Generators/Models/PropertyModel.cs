using Atomic.Injector.Generators.Helpers;
using Atomic.Injector.Generators.Helpers.Templates;

namespace Atomic.Injector.Generators.Models
{
    public class PropertyModel
    {
        public string InterfaceName { get; set; }
        public string ClassName { get; set; }
        public bool IsLazy { get; set; }
        public string PrivateFieldName { get; set; }
        public string PropertyName => PrivateFieldName.ToPascalCase();
        public string[] Dependencies { get; set; }

        private static readonly string _propertyTemplate;
        private static readonly string _propertyInitializationTemplate;
        private static readonly string _constructorInitializationTemplate;

        static PropertyModel()
        {
            _propertyTemplate = ResourcesHelpers.GetTextResource(TemplatePaths.Property);
            _propertyInitializationTemplate = ResourcesHelpers.GetTextResource(TemplatePaths.PropertyInitialization);
            _constructorInitializationTemplate = ResourcesHelpers.GetTextResource(TemplatePaths.ConstructorInitialization);
        }

        public PropertyModel(string interfaceName, string className, string privateFieldName, bool isLazy, string[] dependencies)
        {
            InterfaceName = interfaceName;
            ClassName = className;
            PrivateFieldName = privateFieldName;
            IsLazy = isLazy;
            Dependencies = dependencies;
        }

        public string GetConstructorInitializationString() =>
            GetInitializationString(_constructorInitializationTemplate);

        public string GetPropertyString()
        {
            return _propertyTemplate
                .Replace(Placeholders.ClassName, InterfaceName)
                .Replace(Placeholders.PropertyName, PropertyName)
                .Replace(Placeholders.PrivateFieldName, PrivateFieldName)
                .Replace(Placeholders.Initialization, GetPropertyInitializationString());
        }

        private string GetPropertyInitializationString() => 
            GetInitializationString(_propertyInitializationTemplate);

        private string GetDependenciesString() => string.Join(", ", Dependencies);

        private string GetInitializationString(string template)
        {
            return template
                .Replace(Placeholders.PrivateFieldName, PrivateFieldName)
                .Replace(Placeholders.ClassName, ClassName)
                .Replace(Placeholders.Dependencies, GetDependenciesString());
        }
    }
}
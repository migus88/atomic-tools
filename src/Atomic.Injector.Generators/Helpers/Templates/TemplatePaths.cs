namespace Atomic.Injector.Generators.Helpers.Templates
{
    public static class TemplatePaths
    {
        private const string TemplatesNamespace = "Resources.Templates.";
        private const string TemplatePrefix = TemplatesNamespace + "Template";
        
        public const string Container = TemplatePrefix + "Container";
        public const string PropertyInitialization = TemplatePrefix + "PropertyInitialization";
        public const string ConstructorInitialization = TemplatePrefix + "ConstructorInitialization";
        public const string Property = TemplatePrefix + "Property";
    }
}
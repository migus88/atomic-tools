namespace Atomic.Injector.Generators.Helpers.Templates
{
    //TODO: Auto-generate this class
    public static class TemplatePaths
    {
        private const string TemplatesNamespace = "Resources.Templates.";
        
        public const string Container = TemplatesNamespace + "Container";
        public const string Property = TemplatesNamespace + "Property";
        public const string TransientProperty = TemplatesNamespace + "TransientProperty";
        public const string PropertyInitialization = TemplatesNamespace + "PropertyInitialization";
        public const string ConstructorInitialization = TemplatesNamespace + "ConstructorInitialization";
        public const string TransientInitialization = TemplatesNamespace + "TransientInitialization";

        public static class Scope
        {
            private const string ScopePrefix = TemplatesNamespace + "Scope.";

            public const string Dictionary = ScopePrefix + "Dictionary";
            public const string DictionaryInitialization = ScopePrefix + "DictionaryInitialization";
            public const string Getter = ScopePrefix + "Getter";
            public const string Initialization = ScopePrefix + "Initialization";
        }
    }
}
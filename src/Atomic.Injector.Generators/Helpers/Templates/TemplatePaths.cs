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
            
            public const string FactoriesField = ScopePrefix + "FactoriesField";
            public const string FactoriesInitialization = ScopePrefix + "FactoriesInitialization";
            public const string FactoryGetter = ScopePrefix + "FactoryGetter";
            public const string FieldGetter = ScopePrefix + "FieldGetter";
            public const string FieldNullGetter = ScopePrefix + "FieldNullGetter";
            public const string FieldInitialization = ScopePrefix + "FieldInitialization";
            public const string Getter = ScopePrefix + "Getter";
            public const string InstanceField = ScopePrefix + "InstanceField";
        }
    }
}
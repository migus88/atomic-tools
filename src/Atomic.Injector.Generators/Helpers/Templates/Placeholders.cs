namespace Atomic.Injector.Generators.Helpers.Templates
{
    public static class Placeholders
    {
        private const string OpeningBrackets = "{{";
        private const string ClosingBrackets = "}}";
        
        public const string UsingStatement = OpeningBrackets + "USING" + ClosingBrackets;
        public const string Namespace = OpeningBrackets + "NAMESPACE" + ClosingBrackets;
        public const string ClassName = OpeningBrackets + "CLASS_NAME" + ClosingBrackets;
        public const string Properties = OpeningBrackets + "PROPERTIES" + ClosingBrackets;
        public const string PropertyName = OpeningBrackets + "PROPERTY_NAME" + ClosingBrackets;
        public const string NonLazyInitialization = OpeningBrackets + "NON_LAZY_INITIALIZATION" + ClosingBrackets;
        public const string PrivateFieldName = OpeningBrackets + "PRIVATE_FIELD_NAME" + ClosingBrackets;
        public const string Dependencies = OpeningBrackets + "DEPENDENCIES" + ClosingBrackets;
        public const string Initialization = OpeningBrackets + "INITIALIZATION" + ClosingBrackets;
        public const string Scope = OpeningBrackets + "SCOPE" + ClosingBrackets;
    }
}
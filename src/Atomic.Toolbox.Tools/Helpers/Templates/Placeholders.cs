namespace Atomic.Toolbox.Tools.Helpers.Templates
{
    internal static class Placeholders
    {
        private const string OpeningBrackets = "{{";
        private const string ClosingBrackets = "}}";
        
        public const string UsingStatement = OpeningBrackets + "USING" + ClosingBrackets;
        public const string Namespace = OpeningBrackets + "NAMESPACE" + ClosingBrackets;
        public const string ClassName = OpeningBrackets + "CLASS_NAME" + ClosingBrackets;
        public const string EnumName = OpeningBrackets + "ENUM_NAME" + ClosingBrackets;
        public const string Name = OpeningBrackets + "NAME" + ClosingBrackets;
        public const string Value = OpeningBrackets + "VALUE" + ClosingBrackets;
        public const string Constants = OpeningBrackets + "CONSTS" + ClosingBrackets;
        public const string Extension = OpeningBrackets + "EXTENSION" + ClosingBrackets;
        public const string Initialization = OpeningBrackets + "INITIALIZATION" + ClosingBrackets;
        public const string Constructor = OpeningBrackets + "CONSTRUCTOR" + ClosingBrackets;
        public const string Type = OpeningBrackets + "TYPE" + ClosingBrackets;
        
        
        internal static class Enum
        {
            public const string Values = OpeningBrackets + "VALUES" + ClosingBrackets;
            public const string Names = OpeningBrackets + "NAMES" + ClosingBrackets;
            
            
            public const string ValuesByName = OpeningBrackets + "VALUES_BY_NAME" + ClosingBrackets;
            public const string NamesByValue = OpeningBrackets + "NAMES_BY_VALUE" + ClosingBrackets;
            public const string NamesByEnum = OpeningBrackets + "NAMES_BY_ENUM" + ClosingBrackets;
            public const string EnumsByName = OpeningBrackets + "ENUMS_BY_NAME" + ClosingBrackets;
            public const string EnumsByValue = OpeningBrackets + "ENUMS_BY_VALUE" + ClosingBrackets;
            public const string ValuesByEnum = OpeningBrackets + "VALUES_BY_ENUM" + ClosingBrackets;
        }
    }
}
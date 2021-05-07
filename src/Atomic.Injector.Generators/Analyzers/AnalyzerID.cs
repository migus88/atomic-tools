namespace Atomic.Injector.Generators.Analyzers
{
    public static class AnalyzerID
    {
        public const string Prefix = "ATDI";
        
        public const string DuplicateTransientInstalls = Prefix + "001";
        public const string DuplicateScopedInstalls = Prefix + "002";
        public const string DuplicateSingletonInstalls = Prefix + "003";
        public const string DuplicateScopedIDDeclaration = Prefix + "004";
        
        public const string MissingScopedIDDeclaration = Prefix + "009";
    }
}
namespace Atomic.Injector.Generators.Analyzers
{
    public static class AnalyzerID
    {
        public const string Prefix = "ATDI";
        
        public const string DuplicateTransientInstalls = Prefix + "001";
        public const string DuplicateScopedInstalls = Prefix + "002";
        public const string DuplicateSingletonInstalls = Prefix + "003";
        public const string DuplicateScopedIDDeclaration = Prefix + "004";
        public const string NonPrivateInstallField = Prefix + "005";
        public const string ContainerEmptyConstructor = Prefix + "006";
        public const string ContainerConstructorTrigger = Prefix + "007";
        public const string MultipleDifferentInstallersOnSameField = Prefix + "008";
        public const string DependencyConstructorsInjectAttribute = Prefix + "009";
        public const string MissingScopedIDDeclaration = Prefix + "010";
        public const string RequiredDependencyInstallation = Prefix + "011";
    }
}
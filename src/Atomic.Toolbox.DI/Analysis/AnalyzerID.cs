namespace Atomic.Toolbox.DI.Analyzers
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
        public const string ConstructorParameterInjectID = Prefix + "012";
        public const string TransientInitMode = Prefix + "013";
        public const string MultipleTransientInstallsWithoutID = Prefix + "014";
        public const string MultipleSingletonInstallations = Prefix + "015";
        public const string InterfacesMustHaveImplementation = Prefix + "016";
        public const string DuplicateTransientIDDeclaration = Prefix + "017";
        public const string MultipleInjectConstructors = Prefix + "018";
    }
}
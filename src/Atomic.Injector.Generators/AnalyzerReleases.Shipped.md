## Release 1.0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
ATDI001  | Declaration  | Error    | [DuplicateTransientInstallsAnalyzer](https://google.com) <br> Multiple Transient installs for the same type
ATDI002  | Declaration  | Error    | [DuplicateScopedInstallsAnalyzer](https://google.com) <br> Multiple Scoped installs for the same type
ATDI003  | Declaration  | Error    | [DuplicateSingletonInstallsAnalyzer](https://google.com) <br> Multiple Singleton installs for the same type
ATDI004  | Declaration  | Error    | [DuplicateScopedIDDeclarationAnalyzer](https://google.com) <br> Multiple Scoped installs for the same type with the same ID
ATDI005  | Declaration  | Error    | [NonPrivateInstallFieldAnalyzer](https://google.com) <br> Install field is not private
ATDI006  | Declaration  | Error    | [ContainerEmptyConstructorAnalyzer](https://google.com) <br> Container can't have an empty constructor
ATDI007  | Initialization  | Error    | [ContainerConstructorTriggerAnalyzer](https://google.com) <br> Container doesn't trigger internal constructor
ATDI008  | Declaration  | Error    | [MultipleDifferentInstallersOnSameFieldAnalyzer](https://google.com) <br> Multiple different Install Attributes
ATDI009  | Declaration  | Error    | [DependencyConstructorsInjectAttributeAnalyzer](https://google.com) <br> Inject attribute is missing
ATDI010  | Declaration  | Error    | [MissingScopedIDDeclarationAnalyzer](https://google.com) <br> Missing Id for Scoped installation
ATDI011  | Initialization  | Error    | [RequiredDependencyInstallationAnalyzer](https://google.com) <br> Missing dependency installation
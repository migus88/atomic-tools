using Atomic.Injector.Generators.Enums;

namespace Atomic.Injector.Generators.Definitions
{
    public class InstallDefinition
    {
        public string BoundType { get; set; }
        public bool IsLazy { get; set; }
        public InstallMode Mode { get; set; }
    }
}
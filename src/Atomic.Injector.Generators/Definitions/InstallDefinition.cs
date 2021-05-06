using Atomic.Injector.Generators.Enums;
using Atomic.Injector.Generators.Helpers;

namespace Atomic.Injector.Generators.Definitions
{
    public class InstallDefinition
    {
        public string PropertyName => PrivateFieldName.ToPascalCase();
        
        public string InterfaceName { get; set; }
        public string PrivateFieldName { get; set; }
        public string[] Dependencies { get; set; }
        public string BoundType { get; set; }
        public bool IsLazy { get; set; }
        public InstallMode Mode { get; set; }
        public string ID { get; set; }
    }
}
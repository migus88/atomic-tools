using System.Collections.Generic;
using Atomic.Toolbox.DI.Helpers;
using Atomic.Toolbox.DI.Enums;

namespace Atomic.Toolbox.DI.Generation.Definitions
{
    public class InstallDefinition
    {
        public string PropertyName => PrivateFieldName.ToPascalCase();
        
        public string InterfaceName { get; set; }
        public string PrivateFieldName { get; set; }
        public List<DependencyDefinition> Dependencies { get; set; } = new List<DependencyDefinition>();
        public string BoundType { get; set; }
        public bool IsLazy { get; set; }
        public InstallMode Mode { get; set; }
        public string ID { get; set; }
    }
}
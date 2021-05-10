using Atomic.Toolbox.DI.Enums;
using Atomic.Toolbox.DI.Generation.Models;

namespace Atomic.Toolbox.DI.Generation.Definitions
{
    public class DependencyDefinition
    {
        public InstallMode Mode { get; set; } = InstallMode.Singleton;
        public string TypeName { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string ID { get; set; } = ContainerModel.DefaultID;

        public DependencyDefinition(string typeName = "")
        {
            TypeName = typeName;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(PropertyName))
            {
                return string.Empty;
            }
            
            return Mode.HasFlag(InstallMode.Singleton) ? PropertyName : $"Get{PropertyName}({ID})";
        }
    }
}
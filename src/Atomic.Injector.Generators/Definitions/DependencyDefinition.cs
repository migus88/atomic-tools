using Atomic.Injector.Generators.Enums;

namespace Atomic.Injector.Generators.Definitions
{
    public class DependencyDefinition
    {
        public InstallMode Mode { get; set; }
        public string TypeName { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string ID { get; set; }

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
            
            return Mode.HasFlag(InstallMode.Scoped) ? $"Get{PropertyName}({ID})" : PropertyName;
        }
    }
}
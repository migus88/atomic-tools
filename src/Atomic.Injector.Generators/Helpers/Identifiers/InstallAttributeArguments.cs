using Atomic.Injector.Core.Attributes;
using Atomic.Injector.Core.Enums;

namespace Atomic.Injector.Generators.Helpers.Identifiers
{
    //TODO: Auto-generate this class
    public static class InstallAttributeArguments
    {
        public const string BindTo = nameof(InstallAttribute.BindTo);
        public const string InitMode = nameof(InstallAttribute.InitMode);
        public const string ID = nameof(InstallScopedAttribute.ID);

        public static string ToFullString(this InitMode mode)
        {
            return $"{nameof(Core.Enums.InitMode)}.{mode}";
        }
    }
}
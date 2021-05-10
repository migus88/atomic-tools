using System;
using Atomic.Toolbox.DI.Core.Attributes;

namespace Atomic.Toolbox.DI.Helpers
{
    public static class DiAttributes
    {
        public static readonly Type SingletonType = typeof(InstallSingletonAttribute);
        public static readonly Type ScopedType = typeof(InstallScopedAttribute);
        public static readonly Type TransientType = typeof(InstallTransientAttribute);
        
        public static readonly Type InjectType = typeof(InjectAttribute);

        public static readonly Type[] InstallTypes = new Type[]
        {
            SingletonType,
            ScopedType,
            TransientType
        };
        
        public static class Fields
        {
            public const string BindTo = nameof(InstallAttribute.BindTo);
            public const string InitMode = nameof(InstallAttribute.InitMode);
            public const string ID = nameof(InstallScopedAttribute.ID);
        }
    }
}
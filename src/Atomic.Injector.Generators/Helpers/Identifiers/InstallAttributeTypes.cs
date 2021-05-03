using System;
using Atomic.Injector.Core.Attributes;
using Atomic.Injector.Core.Interfaces;

namespace Atomic.Injector.Generators.Helpers.Identifiers
{
    public static class InstallAttributeTypes
    {
        public static readonly Type ContainerInterfaceType = typeof(IDiContainer);
        public static  readonly Type SingletonAttributeType = typeof(InstallSingletonAttribute);
        public static  readonly Type ScopedAttributeType = typeof(InstallScopedAttribute);
        public static  readonly Type TransientAttributeType = typeof(InstallTransientAttribute);
        public static  readonly Type InjectAttributeType = typeof(InjectAttribute);
    }
}
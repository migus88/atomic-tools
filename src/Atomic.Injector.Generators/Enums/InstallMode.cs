using System;

namespace Atomic.Injector.Generators.Enums
{
    [Flags]
    public enum InstallMode
    {
        Singleton = 1 << 1,
        Scoped = 1 << 2,
        Transient = 1 << 3
    }
}
using System;

namespace Atomic.Toolbox.Tools.Core.Enums
{
    [Flags]
    public enum EnumExtensionMode
    {
        ValuesByName = 1 << 0,
        NamesByValue = 1 << 1,
        NamesByEnum = 1 << 2,
        EnumsByName = 1 << 3,
        EnumsByValue = 1 << 4,
        ValuesByEnum = 1 << 5,
        All = 1 << 31
    }
}
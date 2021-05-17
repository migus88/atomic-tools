using System;
using Atomic.Toolbox.Tools.Core.Enums;

namespace Atomic.Toolbox.Tools.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class ExtendedEnumAttribute : Attribute
    {
        public EnumExtensionMode Mode { get; set; }
    }
}
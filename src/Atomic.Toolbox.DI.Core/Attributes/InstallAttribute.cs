using System;
using Atomic.Toolbox.DI.Core.Enums;

namespace Atomic.Toolbox.DI.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class InstallAttribute : Attribute
    {
        public Type BindTo { get; set; }
        public InitMode InitMode { get; set; } = InitMode.NonLazy;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class InstallSingletonAttribute : InstallAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class InstallScopedAttribute : InstallAttribute
    {
        public string ID { get; set; } = string.Empty;
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class InstallTransientAttribute : InstallScopedAttribute
    {
    }
    
    
    
}
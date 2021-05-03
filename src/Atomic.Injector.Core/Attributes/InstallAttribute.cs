using System;
using Atomic.Injector.Core.Enums;

namespace Atomic.Injector.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public abstract class InstallAttribute : Attribute
    {
        public Type BindTo { get; set; }
        public InitMode InitMode { get; set; } = InitMode.NonLazy;
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class InstallSingletonAttribute : InstallAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class InstallScopedAttribute : InstallAttribute
    {
        public string ID { get; set; } = string.Empty;
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class InstallTransientAttribute : Attribute
    {
        public Type BindTo { get; set; }
        public string ID { get; set; } = string.Empty;
    }
    
    
    
}
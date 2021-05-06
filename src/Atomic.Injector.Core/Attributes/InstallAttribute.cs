using System;
using Atomic.Injector.Core.Enums;

namespace Atomic.Injector.Core.Attributes
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

    //TODO: Implement transient scope
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class InstallTransientAttribute : Attribute
    {
        public Type BindTo { get; set; }
    }
    
    
    
}
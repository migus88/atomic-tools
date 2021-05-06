using System;

namespace Atomic.Injector.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Parameter)]
    public class InjectAttribute : Attribute
    {
        public string ID { get; set; }
    }
}
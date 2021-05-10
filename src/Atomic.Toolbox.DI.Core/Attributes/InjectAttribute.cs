using System;

namespace Atomic.Toolbox.DI.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Parameter)]
    public class InjectAttribute : Attribute
    {
        public string ID { get; set; }
    }
}
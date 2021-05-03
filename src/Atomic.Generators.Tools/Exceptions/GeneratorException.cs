using System;
using System.Runtime.CompilerServices;

namespace Atomic.Generators.Tools.Exceptions
{
    public class GeneratorException : Exception
    {
        public const string DefaultErrorID = "ATOM1";
        public const string DefaultErrorCategory = "AtomicGenerator";
        
        public string Sender { get; }
        public string ID { get; }
        public string Category { get; }
        

        public GeneratorException(string message, string id = DefaultErrorID,
            string category = DefaultErrorCategory, [CallerMemberName]string sender = "") : base(message)
        {
            Sender = sender;
            ID = id;
            Category = category;
        }
    }
}
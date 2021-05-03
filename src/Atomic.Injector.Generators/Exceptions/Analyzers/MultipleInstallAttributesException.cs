using System.Runtime.CompilerServices;
using Atomic.Generators.Tools.Exceptions;

namespace Atomic.Injector.Generators.Exceptions.Analyzers
{
    public class MultipleInstallAttributesException : GeneratorException
    {
        public MultipleInstallAttributesException(string fieldName,
            [CallerMemberName] string sender = "")
            : base($"Field '{fieldName}' has more than one Install Attribute", sender: sender)
        {
        }
    }
}
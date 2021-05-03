using System.Runtime.CompilerServices;
using Atomic.Generators.Tools.Exceptions;

namespace Atomic.Injector.Generators.Exceptions.Analyzers
{
    public class MultipleIdenticalInstallationsException : GeneratorException
    {
        public MultipleIdenticalInstallationsException(string className, string type, [CallerMemberName] string sender = "")
            : base($"'{className}' has multiple identical installations for '{type}'", sender: sender)
        {
        }
    }
}
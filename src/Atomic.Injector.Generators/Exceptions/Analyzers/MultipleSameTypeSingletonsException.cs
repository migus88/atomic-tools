using System.Runtime.CompilerServices;
using Atomic.Generators.Tools.Exceptions;

namespace Atomic.Injector.Generators.Exceptions.Analyzers
{
    public class MultipleSameTypeSingletonsException : GeneratorException
    {
        public MultipleSameTypeSingletonsException(string className, string singletonType, [CallerMemberName] string sender = "")
            : base($"'{className}' has more than one Singleton declaration for '{singletonType}'", sender: sender)
        {
        }
    }
}
using System.Runtime.CompilerServices;
using Atomic.Generators.Tools.Exceptions;

namespace Atomic.Injector.Generators.Exceptions
{
    public class ClassNotFoundException : GeneratorException
    {
        public ClassNotFoundException(string classType, [CallerMemberName]string sender = "") : base($"Can't find class of type: {classType}")
        {
        }
    }
}
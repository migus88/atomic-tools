using System.Runtime.CompilerServices;
using Atomic.Generators.Tools.Exceptions;

namespace Atomic.Injector.Generators.Exceptions.Analyzers
{
    public class DifferentInstallTypesException : GeneratorException
    {
        public DifferentInstallTypesException([CallerMemberName] string sender = "")
            : base($"Field cannot have different 'Install' attributes", sender: sender)
        {
        }
    }
}
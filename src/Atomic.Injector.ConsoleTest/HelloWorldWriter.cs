using Atomic.Injector.ConsoleTest.Interfaces;
using Atomic.Toolbox.DI.Core.Attributes;

namespace Atomic.Injector.ConsoleTest
{
    
    public class HelloWorldWriter
    {
        private readonly IDebugWriter _debugWriter;

        [Inject]
        public HelloWorldWriter(IDebugWriter debugWriter)
        {
            _debugWriter = debugWriter;
        }
    }
}
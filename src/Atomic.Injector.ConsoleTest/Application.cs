using Atomic.Injector.ConsoleTest.Interfaces;
using Atomic.Injector.Core.Attributes;

namespace Atomic.Injector.ConsoleTest
{
    public class Application
    {
        private readonly IDebugWriter _debugWriter;
        private readonly ITrueFalseWriter _trueFalseWriter;

        [Inject]
        public Application(IDebugWriter debugWriter, ITrueFalseWriter trueFalseWriter)
        {
            _debugWriter = debugWriter;
            _trueFalseWriter = trueFalseWriter;
        }

        public void Run()
        {
            _debugWriter.Write("Is this class constructed manually?");
            _trueFalseWriter.WriteFalse();
            _debugWriter.Write("Then it's constructed with DI container?");
            _trueFalseWriter.WriteTrue();
        }
    }
}
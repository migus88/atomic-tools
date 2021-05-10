using Atomic.Injector.ConsoleTest.Interfaces;
using Atomic.Toolbox.DI.Core.Attributes;

namespace Atomic.Injector.ConsoleTest.Implementations
{
    public class TrueFalseWriter : ITrueFalseWriter
    {
        private readonly IDebugWriter _debugWriter;

        [Inject]
        public TrueFalseWriter(IDebugWriter debugWriter)
        {
            _debugWriter = debugWriter;
        }
        
        public void WriteTrue()
        {
            _debugWriter.Write("True");
        }

        public void WriteFalse()
        {
            _debugWriter.Write("False");
        }
    }
}
using Atomic.Injector.ConsoleTest.Implementations;
using Atomic.Injector.ConsoleTest.Interfaces;
using Atomic.Injector.Core.Attributes;
using Atomic.Injector.Core.Enums;
using Atomic.Injector.Core.Interfaces;

namespace Atomic.Injector.ConsoleTest
{
    public partial class DiContainer : IDiContainer
    {
        [InstallSingleton]
        private Application _application;

        [InstallSingleton(InitMode = InitMode.Lazy, BindTo = typeof(DebugWriter))]
        private IDebugWriter _debugWriter;

        [InstallSingleton(BindTo = typeof(TrueFalseWriter))]
        private ITrueFalseWriter _trueFalseWriter;

        [InstallTransient(BindTo = typeof(TrueFalseWriter))]
        private ITrueFalseWriter _testTrueFalseWriter;

        [InstallTransient(BindTo = typeof(TrueFalseWriter))]
        private ITrueFalseWriter _test2TrueFalseWriter;

        [InstallTransient(BindTo = typeof(TrueFalseWriter))]
        private ITrueFalseWriter _test3TrueFalseWriter;

        public void Init()
        {
            _application.Run();
        }
    }
}
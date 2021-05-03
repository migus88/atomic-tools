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

        [InstallScoped(ID = "Test", BindTo = typeof(TrueFalseWriter))]
        private ITrueFalseWriter _testTrueFalseWriter;

        [InstallScoped(ID = "Test2", BindTo = typeof(TrueFalseWriter), InitMode = InitMode.Lazy)]
        private ITrueFalseWriter _test2TrueFalseWriter;

        [InstallScoped(ID = "Test3", BindTo = typeof(TrueFalseWriter), InitMode = InitMode.Lazy)]
        private ITrueFalseWriter _test3TrueFalseWriter;

        public void Init()
        {
            _application.Run();
        }
    }
}
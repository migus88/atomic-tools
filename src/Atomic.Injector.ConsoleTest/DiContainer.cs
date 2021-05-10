using Atomic.Injector.ConsoleTest.Implementations;
using Atomic.Injector.ConsoleTest.Interfaces;
using Atomic.Toolbox.DI.Core.Attributes;
using Atomic.Toolbox.DI.Core.Enums;
using Atomic.Toolbox.DI.Core.Interfaces;

namespace Atomic.Injector.ConsoleTest
{
    public partial class DiContainer : IDiContainer
    {
        
        [InstallSingleton]
        private Application _application;
        
        [InstallScoped(ID = TestClass.TestConst, BindTo = typeof(TrueFalseWriter))]
        [InstallScoped(ID = "Test2", BindTo = typeof(TrueFalseWriter), InitMode = InitMode.Lazy)]
        private ITrueFalseWriter _scopedTrueFalseWriter;
        
        [InstallTransient(BindTo = typeof(DebugWriter), ID = "d")]
        [InstallTransient(BindTo = typeof(DebugWriter), ID = "d1", InitMode = InitMode.Lazy)]
        [InstallTransient(BindTo = typeof(DebugWriter))]
        private IDebugWriter _debugWriter2;

        public void Init()
        {
            _application.Run();
            
        }

        public DiContainer(string t) : this()
        {
            this._application = new Application(null, null);
        }
    }
}
public class TestClass
{
        
    public const string TestConst = "sdf";
}
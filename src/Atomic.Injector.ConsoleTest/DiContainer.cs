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
        
        //TODO: handle const assignment of ID
        [InstallScoped(ID = "Test", BindTo = typeof(TrueFalseWriter))]
        [InstallScoped(ID = "Test2", BindTo = typeof(TrueFalseWriter), InitMode = InitMode.Lazy)]
        private ITrueFalseWriter _scopedTrueFalseWriter;
        
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
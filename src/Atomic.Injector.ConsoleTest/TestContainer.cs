/*using System;
using System.Collections.Generic;
using Atomic.Injector.ConsoleTest.Implementations;
using Atomic.Injector.ConsoleTest.Interfaces;

namespace Atomic.Injector.ConsoleTest
{
    public sealed partial class DiContainerTest
    {
        
        private Application _application;
        private IDebugWriter _debugWriter;
        private ITrueFalseWriter _trueFalseWriter;

        public Application SingletonApplication => _application ?? (_application = new Application(null, null));
        public Application TransientApplication => new Application(null, null);
        public Atomic.Injector.ConsoleTest.Interfaces.ITrueFalseWriter TrueFalseWriter => _trueFalseWriter != default ? _trueFalseWriter : (_trueFalseWriter = new Atomic.Injector.ConsoleTest.Implementations.TrueFalseWriter(DebugWriter));


        //Scoped only
        private readonly Dictionary<string, Application> _applicationScopes;
        private readonly Dictionary<string, Func<Application>> _applicationScopeFactories;

        public Application GetApplication(string id) => _applicationScopeFactories[id]();

        public DiContainerTest()
        {
            _applicationScopeFactories = new Dictionary<string, Func<Application>>()
            {
                ["NonLazyTest"] = () => _applicationScopes["NonLazyTest"] == default ? (_applicationScopes["NonLazyTest"] = new Application(null, TrueFalseWriter)) : _applicationScopes["NonLazyTest"], 
                ["Lazy"] = null,
            };
            _applicationScopes = new Dictionary<string, Application>()
            {
                ["NonLazyTest"] = this.GetApplication("NonLazyTest"), //NonLazy
                ["Lazy"] = null,
            };
        }
    }
}*/
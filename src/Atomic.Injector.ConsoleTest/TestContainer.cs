/*using System;
using System.Collections.Generic;
using Atomic.Injector.ConsoleTest.Implementations;
using Atomic.Injector.ConsoleTest.Interfaces;

namespace Atomic.Injector.ConsoleTest
{
    public sealed partial class DiContainer
    {
        private Application Application =>  _application ?? (_application = new Application(DebugWriter, TrueFalseWriter));
        private IDebugWriter DebugWriter => _debugWriter ?? (_debugWriter = new DebugWriter());
        private ITrueFalseWriter TrueFalseWriter => _trueFalseWriter ?? (_trueFalseWriter = new TrueFalseWriter(DebugWriter));
        private ITrueFalseWriter TestTrueFalseWriter => _testTrueFalseWriter ?? (_testTrueFalseWriter = new TrueFalseWriter(DebugWriter));
        private ITrueFalseWriter Test2TrueFalseWriter => _test2TrueFalseWriter ?? (_test2TrueFalseWriter = new TrueFalseWriter(DebugWriter)); 
        
        public DiContainer()
        {
            _application = new Application(DebugWriter, TrueFalseWriter);
            _trueFalseWriter = new TrueFalseWriter(DebugWriter);
            _testTrueFalseWriter = new TrueFalseWriter(DebugWriter);
        }
    }
}*/
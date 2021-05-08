using System;
using Atomic.Injector.ConsoleTest.Interfaces;
using Atomic.Injector.Core.Attributes;

namespace Atomic.Injector.ConsoleTest.Implementations
{
    public class DebugWriter : IDebugWriter
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }
}
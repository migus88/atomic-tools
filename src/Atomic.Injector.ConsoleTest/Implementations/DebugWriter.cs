using System;
using Atomic.Injector.ConsoleTest.Interfaces;

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
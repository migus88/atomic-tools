using System;

namespace Atomic.Injector.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new DiContainer();
            container.Init();
        }
    }
}
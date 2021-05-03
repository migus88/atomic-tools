using System.IO;
using System.Reflection;

namespace Atomic.Injector.Generators.Helpers
{
    public static class ResourcesHelpers
    {
        private static Assembly _assembly;
        private static string _defaultNamespace;

        static ResourcesHelpers()
        {
            _assembly = Assembly.GetExecutingAssembly();
            _defaultNamespace = _assembly.ManifestModule.ScopeName.Replace(".dll", "");
        }
        
        public static string GetTextResource(string name)
        {
            var fullName = $"{_defaultNamespace}.{name}.txt";

            using var stream = _assembly.GetManifestResourceStream(fullName);

            if (stream == null)
            {
                return null;
            }
            
            using var reader = new StreamReader(stream);
            
            return reader.ReadToEnd();
        }
    }
}
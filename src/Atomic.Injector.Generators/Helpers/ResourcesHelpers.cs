using System.IO;
using System.Reflection;

namespace Atomic.Injector.Generators.Helpers
{
    public static class ResourcesHelpers
    {
        public static string GetResourceText(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var defaultNamespace = assembly.ManifestModule.ScopeName.Replace(".dll", "");
            var fullName = $"{defaultNamespace}.Resources.Templates.{name}.txt";

            using var stream = assembly.GetManifestResourceStream(fullName);

            if (stream == null)
            {
                return null;
            }
            
            using var reader = new StreamReader(stream);
            
            return reader.ReadToEnd();
        }
    }
}
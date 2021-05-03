namespace Atomic.Generators.Tools.Helpers
{
    public static class HelperMethods
    {
        public static string GetFullClassName(string @namespace, string className) => $"{@namespace}.{className}";
    }
}
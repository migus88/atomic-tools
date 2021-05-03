using System.Linq;
using Atomic.Injector.Generators.Helpers;

namespace Atomic.Injector.Generators.Templates.Models
{
    public class ContainerModel
    {
        public string UsingStatement { get; set; }
        public string Namespace { get; set; }
        public string ClassName { get; set; }
        public PropertyModel[] Properties { get; set; }

        private static readonly string _template;

        static ContainerModel()
        {
            _template = ResourcesHelpers.GetResourceText(TemplatePaths.Container);
        }


        public ContainerModel(string usingStatement, string @namespace, string className, PropertyModel[] properties)
        {
            UsingStatement = usingStatement;
            Namespace = @namespace;
            ClassName = className;
            Properties = properties;
        }

        public override string ToString()
        {
            return _template
                .Replace(Placeholders.UsingStatement, UsingStatement)
                .Replace(Placeholders.Namespace, Namespace)
                .Replace(Placeholders.ClassName, ClassName)
                .Replace(Placeholders.PrivateProperties, GetPrivatePropertiesString())
                .Replace(Placeholders.NonLazyInitialization, GetConstructorInitializationString());
        }

        private string GetPrivatePropertiesString() =>
            string.Join("\r\n\t\t", Properties.Select(p => p.GetPropertyString()).ToList());

        private string GetConstructorInitializationString() =>
            string.Join("\r\n\t\t\t",
                Properties.Where(p => !p.IsLazy)
                    .Select(p => p.GetConstructorInitializationString()).ToList());
    }
}
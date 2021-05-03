using System.Linq;
using Atomic.Injector.Generators.Helpers;
using Atomic.Injector.Generators.Helpers.Templates;

namespace Atomic.Injector.Generators.Models
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
            _template = ResourcesHelpers.GetTextResource(TemplatePaths.Container);
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
                .Replace(Placeholders.Properties, GetPropertiesString())
                .Replace(Placeholders.NonLazyInitialization, GetConstructorInitializationString());
        }

        private string GetPropertiesString() =>
            string.Join("\r\n\t\t", Properties.Select(p => p.GetPropertyString()).ToList());

        private string GetConstructorInitializationString() =>
            string.Join("\r\n\t\t\t",
                Properties.Where(p => !p.InstallModel.IsLazy)
                    .Select(p => p.GetConstructorInitializationString()).ToList());
    }
}
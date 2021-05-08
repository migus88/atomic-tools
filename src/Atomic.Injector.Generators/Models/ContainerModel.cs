using System.Linq;
using Atomic.Injector.Generators.Helpers;
using Atomic.Injector.Generators.Helpers.Templates;
using Atomic.Injector.Generators.Interfaces;

namespace Atomic.Injector.Generators.Models
{
    public class ContainerModel
    {
        public const string DefaultID = "DefaultID";
        
        private const string PropertySeparator = "\r\n\t\t";
        private const string PrivateFieldSeparator = "\r\n\t\t";
        private const string ConstructorSeparator = "\r\n";
        private const string MethodSeparator = "\r\n\r\n\t\t";
        
        public string UsingStatement { get; }
        public string Namespace { get; }
        public string ClassName { get; }
        public IFieldModel[] FieldModels { get; }

        private static readonly string _template;

        static ContainerModel()
        {
            _template = ResourcesHelpers.GetTextResource(TemplatePaths.Container);
        }


        public ContainerModel(string usingStatement, string @namespace, string className, IFieldModel[] fieldModels)
        {
            UsingStatement = usingStatement;
            Namespace = @namespace;
            ClassName = className;
            FieldModels = fieldModels;
        }

        public override string ToString()
        {
            return _template
                .Replace(Placeholders.UsingStatement, UsingStatement)
                .Replace(Placeholders.Namespace, Namespace)
                .Replace(Placeholders.ClassName, ClassName)
                .Replace(Placeholders.DefaultScopeID, DefaultID)
                .Replace(Placeholders.Properties, GetPropertiesString())
                .Replace(Placeholders.Methods, GetMethodsString())
                .Replace(Placeholders.PrivateFields, GetPrivateFieldsString())
                .Replace(Placeholders.NonLazyInitialization, GetConstructorInitializationString());
        }

        private string GetPropertiesString()
        {
            var propertyStrings =
                from model in FieldModels
                let propertyString = model.GetPropertyString()
                where !string.IsNullOrEmpty(propertyString)
                select propertyString;
            
            return string.Join(PropertySeparator, propertyStrings);
        }

        private string GetMethodsString()
        {
            var methodsStrings =
                from model in FieldModels
                let methodString = model.GetMethodString()
                where !string.IsNullOrEmpty(methodString)
                select methodString;
            
            return string.Join(MethodSeparator, methodsStrings);
        }

        private string GetPrivateFieldsString()
        {
            var fieldsStrings =
                from model in FieldModels
                let fieldString = model.GetPrivateFieldString()
                where !string.IsNullOrEmpty(fieldString)
                select fieldString;
            
            return string.Join(PrivateFieldSeparator, fieldsStrings);
        }

        private string GetConstructorInitializationString()
        {
            var constructorStrings =
                from model in FieldModels
                let constructorString = model.GetConstructorString()
                where !string.IsNullOrEmpty(constructorString)
                select constructorString;
            
            return string.Join(ConstructorSeparator, constructorStrings);
        }
    }
}
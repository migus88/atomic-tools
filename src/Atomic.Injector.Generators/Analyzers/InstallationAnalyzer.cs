using System.Collections.Generic;
using System.Linq;
using Atomic.Generators.Tools.Parsers;
using Atomic.Injector.Generators.Exceptions.Analyzers;
using Atomic.Injector.Generators.Helpers.Identifiers;
using static Atomic.Injector.Generators.Helpers.Identifiers.InstallAttributeTypes;

namespace Atomic.Injector.Generators.Analyzers
{
    public class InstallationAnalyzer
    {
        private readonly Class _class;

        public InstallationAnalyzer(Class @class)
        {
            _class = @class;
        }

        public void AnalyzeAll()
        {
            AnalyzeInstallFields();
        }

        public void AnalyzeInstallFields()
        {
            var fields = _class.GetFieldsWithAttributes();

            foreach (var field in fields)
            {
                AnalyzeFieldsWithIdenticalType(field);
            }
        }

        private void AnalyzeFieldsWithIdenticalType(Field field)
        {
            var sameTypeFields = _class.Fields
                .Where(f => f.Name != field.Name && f.TypeName == field.TypeName)
                .ToList();

            if (sameTypeFields.Count == 0)
            {
                return;
            }

            ValidateNoMultipleSingletonsOfSameType(field, sameTypeFields);
            ValidateNoIdenticalInstallations(field, sameTypeFields);
        }

        private void ValidateNoIdenticalInstallations(Field field, List<Field> sameTypeFields)
        {
            var installAttributes = field.GetAttributes(ScopedAttributeType, TransientAttributeType);

            if (installAttributes.Count == 0)
            {
                return;
            }

            if (installAttributes.Count > 1)
            {
                throw new MultipleInstallAttributesException(field.Name);
            }

            var installAttribute = installAttributes.First();
            var idArgument = installAttribute.GetArgument(InstallAttributeArguments.ID);


            foreach (var sameTypeField in sameTypeFields)
            {
                var sameTypeAttribute = sameTypeField.GetAttributes(ScopedAttributeType, TransientAttributeType)
                    .FirstOrDefault();

                if (sameTypeAttribute == null)
                {
                    continue;
                }

                var sameTypeAttributeIdArgument = sameTypeAttribute.GetArgument(InstallAttributeArguments.ID);

                if (sameTypeAttributeIdArgument?.Value == idArgument?.Value)
                {
                    throw new MultipleIdenticalInstallationsException(_class.ClassName, field.TypeName);
                }
            }
        }

        private void ValidateNoMultipleSingletonsOfSameType(Field field, List<Field> sameTypeFields)
        {
            if (field.HasAttribute(SingletonAttributeType) &&
                sameTypeFields.Any(f => f.HasAttribute(SingletonAttributeType)))
            {
                throw new MultipleSameTypeSingletonsException(_class.ClassName, field.TypeName);
            }
        }
    }
}
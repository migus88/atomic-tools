using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Atomic.Injector.Generators.Definitions;
using Atomic.Injector.Generators.Enums;
using Atomic.Injector.Generators.Helpers;
using Atomic.Injector.Generators.Helpers.Templates;

namespace Atomic.Injector.Generators.Models
{
    //TODO: Handle scope destruction
    public class ScopedFieldModel : BaseFieldModel
    {
        private static readonly string _factoriesFieldTemplate;
        private static readonly string _factoriesInitializationTemplate;
        private static readonly string _factoryGetterTemplate;
        private static readonly string _fieldGetterTemplate;
        private static readonly string _fieldNullGetterTemplate;
        private static readonly string _fieldInitializationTemplate;
        private static readonly string _getterTemplate;
        private static readonly string _instanceFieldTemplate;

        static ScopedFieldModel()
        {
            _factoriesFieldTemplate = ResourcesHelpers.GetTextResource(TemplatePaths.Scope.FactoriesField);
            _factoryGetterTemplate = ResourcesHelpers.GetTextResource(TemplatePaths.Scope.FactoryGetter);
            _fieldGetterTemplate = ResourcesHelpers.GetTextResource(TemplatePaths.Scope.FieldGetter);
            _fieldInitializationTemplate = ResourcesHelpers.GetTextResource(TemplatePaths.Scope.FieldInitialization);
            _getterTemplate = ResourcesHelpers.GetTextResource(TemplatePaths.Scope.Getter);
            _instanceFieldTemplate = ResourcesHelpers.GetTextResource(TemplatePaths.Scope.InstanceField);
            _fieldNullGetterTemplate = ResourcesHelpers.GetTextResource(TemplatePaths.Scope.FieldNullGetter);
            _factoriesInitializationTemplate =
                ResourcesHelpers.GetTextResource(TemplatePaths.Scope.FactoriesInitialization);
        }

        public ScopedFieldModel(List<InstallDefinition> installDefinitions) : base(installDefinitions)
        {
        }

        public override string GetPrivateFieldString()
        {
            return
                $"{GetFactoriesField()}{LineBreakSymbol}{TabSymbol}{TabSymbol}{GetInstanceField()}{LineBreakSymbol}{LineBreakSymbol}";
        }

        public override string GetMethodString()
        {
            var generatedCode = _getterTemplate
                .Replace(Placeholders.ClassName, _firstDefinition.InterfaceName)
                .Replace(Placeholders.PropertyName, _firstDefinition.PropertyName)
                .Replace(Placeholders.PrivateFieldName, _firstDefinition.PrivateFieldName);

            return $"{generatedCode}{LineBreakSymbol}{LineBreakSymbol}";
        }

        public override string GetConstructorString()
        {
            return $"{TabSymbol}{TabSymbol}{TabSymbol}" +
                   $"{GetFactoriesInitialization()}{LineBreakSymbol}" +
                   $"{TabSymbol}{TabSymbol}{TabSymbol}" +
                   $"{GetFieldsInitialization()}{LineBreakSymbol}";
        }

        private string GetFieldsInitialization() => _fieldInitializationTemplate
            .Replace(Placeholders.PrivateFieldName, _firstDefinition.PrivateFieldName)
            .Replace(Placeholders.ClassName, _firstDefinition.InterfaceName)
            .Replace(Placeholders.Initialization, GetFieldsGetters());

        private string GetFieldsGetters()
        {
            var builder = new StringBuilder();

            foreach (var installDefinition in _installDefinitions)
            {
                builder.AppendLine(
                    $"{TabSymbol}{TabSymbol}{TabSymbol}{TabSymbol}{GetFieldGetter(installDefinition)}");
            }

            var str = builder.ToString();
            return str.Remove(str.LastIndexOf(Environment.NewLine, StringComparison.Ordinal));
        }

        private string GetFieldGetter(InstallDefinition installDefinition) =>
            (installDefinition.IsLazy ? _fieldNullGetterTemplate : _fieldGetterTemplate)
            .Replace(Placeholders.Scope, installDefinition.ID)
            .Replace(Placeholders.PropertyName, installDefinition.PropertyName);

        private string GetFactoriesInitialization() => _factoriesInitializationTemplate
            .Replace(Placeholders.PrivateFieldName, _firstDefinition.PrivateFieldName)
            .Replace(Placeholders.ClassName, _firstDefinition.InterfaceName)
            .Replace(Placeholders.Initialization, GetFactoriesGetters());

        private string GetFactoriesGetters()
        {
            var builder = new StringBuilder();

            foreach (var installDefinition in _installDefinitions)
            {
                builder.AppendLine(
                    $"{TabSymbol}{TabSymbol}{TabSymbol}{TabSymbol}{GetFactoryGetter(installDefinition)}");
            }

            var str = builder.ToString();
            return str.Remove(str.LastIndexOf(Environment.NewLine, StringComparison.Ordinal));
        }

        private string GetFactoryGetter(InstallDefinition installDefinition) => _factoryGetterTemplate
            .Replace(Placeholders.Scope, installDefinition.ID)
            .Replace(Placeholders.PrivateFieldName, installDefinition.PrivateFieldName)
            .Replace(Placeholders.ClassName, installDefinition.BoundType)
            .Replace(Placeholders.Dependencies, GetDependenciesString(installDefinition.Dependencies));

        private string GetFactoriesField() => _factoriesFieldTemplate
            .Replace(Placeholders.ClassName, _firstDefinition.InterfaceName)
            .Replace(Placeholders.PrivateFieldName, _firstDefinition.PrivateFieldName);

        private string GetInstanceField() => _instanceFieldTemplate
            .Replace(Placeholders.ClassName, _firstDefinition.InterfaceName)
            .Replace(Placeholders.PrivateFieldName, _firstDefinition.PrivateFieldName);
    }
}
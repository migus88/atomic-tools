using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Atomic.Toolbox.DI.Enums;
using Atomic.Toolbox.DI.Generation.Definitions;
using Atomic.Toolbox.DI.Generation.Models.Fields;
using Atomic.Toolbox.DI.Helpers;
using Atomic.Toolbox.DI.Helpers.Templates;

namespace Atomic.Toolbox.DI.Models
{
    //TODO: Handle scope destruction
    public class ScopedFieldModel : BaseFieldModel
    {
        protected static readonly string _factoriesFieldTemplate;
        protected static readonly string _factoriesInitializationTemplate;
        protected static readonly string _factoryGetterTemplate;
        protected static readonly string _fieldGetterTemplate;
        protected static readonly string _fieldNullGetterTemplate;
        protected static readonly string _fieldInitializationTemplate;
        protected static readonly string _getterTemplate;
        protected static readonly string _instanceFieldTemplate;

        static ScopedFieldModel()
        {
            _factoriesFieldTemplate = ResourceLoader.GetTextResource(TemplatePaths.Scope.FactoriesField);
            _factoryGetterTemplate = ResourceLoader.GetTextResource(TemplatePaths.Scope.FactoryGetter);
            _fieldGetterTemplate = ResourceLoader.GetTextResource(TemplatePaths.Scope.FieldGetter);
            _fieldInitializationTemplate = ResourceLoader.GetTextResource(TemplatePaths.Scope.FieldInitialization);
            _getterTemplate = ResourceLoader.GetTextResource(TemplatePaths.Scope.Getter);
            _instanceFieldTemplate = ResourceLoader.GetTextResource(TemplatePaths.Scope.InstanceField);
            _fieldNullGetterTemplate = ResourceLoader.GetTextResource(TemplatePaths.Scope.FieldNullGetter);
            _factoriesInitializationTemplate =
                ResourceLoader.GetTextResource(TemplatePaths.Scope.FactoriesInitialization);
        }

        public ScopedFieldModel(List<InstallDefinition> installDefinitions) : base(installDefinitions)
        {
        }

        public override string GetPrivateFieldString()
        {
            return
                $"{GetFactoriesField()}{LineBreakSymbol}{TabSymbol}{TabSymbol}{GetInstanceField()}{LineBreakSymbol}";
        }

        public override string GetMethodString()
        {
            return _getterTemplate
                .Replace(Placeholders.ClassName, _firstDefinition.InterfaceName)
                .Replace(Placeholders.PropertyName, _firstDefinition.PropertyName)
                .Replace(Placeholders.PrivateFieldName, _firstDefinition.PrivateFieldName);
        }

        public override string GetConstructorString()
        {
            return $"{TabSymbol}{TabSymbol}{TabSymbol}" +
                   $"{GetFactoriesInitialization()}{LineBreakSymbol}" +
                   $"{TabSymbol}{TabSymbol}{TabSymbol}" +
                   $"{GetFieldsInitialization()}{LineBreakSymbol}";
        }

        protected virtual string GetFieldsInitialization() => _fieldInitializationTemplate
            .Replace(Placeholders.PrivateFieldName, _firstDefinition.PrivateFieldName)
            .Replace(Placeholders.ClassName, _firstDefinition.InterfaceName)
            .Replace(Placeholders.Initialization, GetFieldsGetters());

        protected virtual string GetFieldsGetters()
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

        protected virtual string GetFieldGetter(InstallDefinition installDefinition) =>
            (installDefinition.IsLazy ? _fieldNullGetterTemplate : _fieldGetterTemplate)
            .Replace(Placeholders.Scope, installDefinition.ID)
            .Replace(Placeholders.PropertyName, installDefinition.PropertyName);

        protected virtual string GetFactoriesInitialization() => _factoriesInitializationTemplate
            .Replace(Placeholders.PrivateFieldName, _firstDefinition.PrivateFieldName)
            .Replace(Placeholders.ClassName, _firstDefinition.InterfaceName)
            .Replace(Placeholders.Initialization, GetFactoriesGetters());

        protected virtual string GetFactoriesGetters()
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

        protected virtual string GetFactoryGetter(InstallDefinition installDefinition) => _factoryGetterTemplate
            .Replace(Placeholders.Scope, installDefinition.ID)
            .Replace(Placeholders.PrivateFieldName, installDefinition.PrivateFieldName)
            .Replace(Placeholders.ClassName, installDefinition.BoundType)
            .Replace(Placeholders.Dependencies, GetDependenciesString(installDefinition.Dependencies));

        protected virtual string GetFactoriesField() => _factoriesFieldTemplate
            .Replace(Placeholders.ClassName, _firstDefinition.InterfaceName)
            .Replace(Placeholders.PrivateFieldName, _firstDefinition.PrivateFieldName);

        protected virtual string GetInstanceField() => _instanceFieldTemplate
            .Replace(Placeholders.ClassName, _firstDefinition.InterfaceName)
            .Replace(Placeholders.PrivateFieldName, _firstDefinition.PrivateFieldName);
    }
}
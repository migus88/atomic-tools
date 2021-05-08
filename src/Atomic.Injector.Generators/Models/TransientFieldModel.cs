using System;
using System.Collections.Generic;
using Atomic.Injector.Generators.Definitions;
using Atomic.Injector.Generators.Enums;
using Atomic.Injector.Generators.Helpers;
using Atomic.Injector.Generators.Helpers.Templates;
using Atomic.Injector.Generators.Interfaces;

namespace Atomic.Injector.Generators.Models
{
    public class TransientFieldModel : ScopedFieldModel
    {
        private static readonly string _propertyInitializationTemplate;

        static TransientFieldModel()
        {
            _propertyInitializationTemplate =
                ResourcesHelpers.GetTextResource(TemplatePaths.Transient.TransientInitialization);
        }

        public TransientFieldModel(List<InstallDefinition> installDefinitions) : base(installDefinitions)
        {
        }
        
        public override string GetPrivateFieldString()
        {
            return
                $"{GetFactoriesField()}{LineBreakSymbol}";
        }

        public override string GetConstructorString()
        {
            return $"{TabSymbol}{TabSymbol}{TabSymbol}" +
                   $"{GetFactoriesInitialization()}{LineBreakSymbol}";
        }
        
        protected override string GetFactoryGetter(InstallDefinition installDefinition) => _propertyInitializationTemplate
            .Replace(Placeholders.Scope, installDefinition.ID)
            .Replace(Placeholders.ClassName, installDefinition.BoundType)
            .Replace(Placeholders.Dependencies, GetDependenciesString(installDefinition.Dependencies));
    }
}
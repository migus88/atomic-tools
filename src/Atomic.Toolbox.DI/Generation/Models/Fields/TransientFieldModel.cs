using System.Collections.Generic;
using Atomic.Toolbox.DI.Helpers;
using Atomic.Toolbox.DI.Generation.Definitions;
using Atomic.Toolbox.DI.Helpers.Templates;
using Atomic.Toolbox.DI.Models;

namespace Atomic.Toolbox.DI.Generation.Models.Fields
{
    public class TransientFieldModel : ScopedFieldModel
    {
        private static readonly string _propertyInitializationTemplate;

        static TransientFieldModel()
        {
            _propertyInitializationTemplate =
                ResourceLoader.GetTextResource(TemplatePaths.Transient.TransientInitialization);
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
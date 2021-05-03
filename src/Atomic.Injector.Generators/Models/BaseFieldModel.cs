using System;
using Atomic.Injector.Generators.Definitions;
using Atomic.Injector.Generators.Enums;
using Atomic.Injector.Generators.Helpers;
using Atomic.Injector.Generators.Interfaces;

namespace Atomic.Injector.Generators.Models
{
    public abstract class BaseFieldModel : IFieldModel
    {
        protected string _interfaceName;
        protected string _className;
        protected string _privateFieldName;
        protected string[] _dependencies;
        protected InstallDefinition _installDefinition;
        
        protected string PropertyName => _privateFieldName.ToPascalCase();
        
        public BaseFieldModel(string interfaceName, string className, string privateFieldName, InstallDefinition installDefinition, string[] dependencies)
        {
            _interfaceName = interfaceName;
            _className = className;
            _privateFieldName = privateFieldName;
            _installDefinition = installDefinition;
            _dependencies = dependencies;
        }

        public virtual string GetPropertyString() => string.Empty;
        public virtual string GetPrivateFieldString() => string.Empty;
        public virtual string GetMethodString() => string.Empty;
        public virtual string GetConstructorString() => string.Empty;

        public static IFieldModel GetFieldModel(string interfaceName, string className, string privateFieldName,
            InstallDefinition installDefinition, string[] dependencies)
        {
            return installDefinition.Mode switch
            {
                InstallMode.Singleton => new SingletonFieldModel(interfaceName, className, privateFieldName,
                    installDefinition, dependencies),
                InstallMode.Transient => new TransientFieldModel(interfaceName, className, privateFieldName,
                    installDefinition, dependencies),
                InstallMode.Scoped => new ScopedFieldModel(interfaceName, className, privateFieldName,
                    installDefinition, dependencies),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        protected string GetDependenciesString() => string.Join(", ", _dependencies);
    }
}
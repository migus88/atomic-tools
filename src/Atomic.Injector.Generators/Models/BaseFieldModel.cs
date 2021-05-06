using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomic.Injector.Generators.Definitions;
using Atomic.Injector.Generators.Enums;
using Atomic.Injector.Generators.Exceptions.Analyzers;
using Atomic.Injector.Generators.Helpers;
using Atomic.Injector.Generators.Interfaces;

namespace Atomic.Injector.Generators.Models
{
    public abstract class BaseFieldModel : IFieldModel
    {
        protected const string LineBreakSymbol = "\r\n";
        protected const string TabSymbol = "\t";

        protected readonly InstallDefinition _firstDefinition;
        protected readonly List<InstallDefinition> _installDefinitions;

        public BaseFieldModel(List<InstallDefinition> installDefinitions)
        {
            //TODO: Reuse code from validator
            if (installDefinitions.Select(d => d.Mode).Distinct().Count() > 1)
            {
                throw new DifferentInstallTypesException();
            }

            _installDefinitions = installDefinitions;
            _firstDefinition = _installDefinitions.First();
        }

        public virtual string GetPropertyString() => string.Empty;
        public virtual string GetPrivateFieldString() => string.Empty;
        public virtual string GetMethodString() => string.Empty;
        public virtual string GetConstructorString() => string.Empty;

        public static IFieldModel GetFieldModel(List<InstallDefinition> installDefinitions)
        {
            //TODO: Reuse code from validator
            if (installDefinitions.Select(d => d.Mode).Distinct().Count() > 1)
            {
                throw new DifferentInstallTypesException();
            }

            var firstDefinition = installDefinitions.First();

            return firstDefinition.Mode switch
            {
                InstallMode.Singleton => new SingletonFieldModel(installDefinitions),
                InstallMode.Transient => new TransientFieldModel(installDefinitions),
                InstallMode.Scoped => new ScopedFieldModel(installDefinitions),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected static string GetDependenciesString(string[] dependencies) =>
            dependencies == null || dependencies.Length == 0 ? string.Empty : string.Join(", ", dependencies);

        protected static string GetTabSymbols(int count)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                builder.Append(TabSymbol);
            }

            return builder.ToString();
        }
    }
}
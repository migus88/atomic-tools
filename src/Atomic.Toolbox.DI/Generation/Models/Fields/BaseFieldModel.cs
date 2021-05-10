using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomic.Toolbox.DI.Enums;
using Atomic.Toolbox.DI.Generation.Definitions;
using Atomic.Toolbox.DI.Models;

namespace Atomic.Toolbox.DI.Generation.Models.Fields
{
    public abstract class BaseFieldModel : IFieldModel
    {
        protected const string LineBreakSymbol = "\r\n";
        protected const string TabSymbol = "\t";

        protected readonly InstallDefinition _firstDefinition;
        protected readonly List<InstallDefinition> _installDefinitions;

        public BaseFieldModel(List<InstallDefinition> installDefinitions)
        {
            if (installDefinitions.Select(d => d.Mode).Distinct().Count() > 1)
            {
                throw new Exception("Multiple different install types");
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
            var firstDefinition = installDefinitions.First();

            return firstDefinition.Mode switch
            {
                InstallMode.Singleton => new SingletonFieldModel(installDefinitions),
                InstallMode.Transient => new TransientFieldModel(installDefinitions),
                InstallMode.Scoped => new ScopedFieldModel(installDefinitions),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected static string GetDependenciesString(IEnumerable<DependencyDefinition> dependencies)
        {
            var dependencyStrings = dependencies.Select(d => d.ToString()).ToArray();
            return string.Join(", ", dependencyStrings);
        }

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
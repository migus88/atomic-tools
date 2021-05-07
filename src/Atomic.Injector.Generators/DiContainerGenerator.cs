using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Atomic.Generators.Tools.Exceptions;
using Atomic.Injector.Core.Attributes;
using Atomic.Injector.Core.Interfaces;
using Atomic.Injector.Generators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Atomic.Generators.Tools.Helpers;
using Atomic.Generators.Tools.Parsers;
using Atomic.Injector.Generators.Exceptions;
using Atomic.Injector.Generators.Models;
using Attribute = Atomic.Generators.Tools.Parsers.Attribute;

namespace Atomic.Injector.Generators
{
    [Generator]
    public class DiContainerGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
/*
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
*/
#endif
        }

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                HandleContext(context);
            }
            catch(Exception ex)
            {
                var diagnostic = Diagnostic.Create(
                    new DiagnosticDescriptor("ATOM99", $"Container Generation didn't run",
                        "Container Generation didn't run. For more info, look for errors.", "Container Generation",
                        DiagnosticSeverity.Warning, true),
                    null);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void HandleContext(GeneratorExecutionContext context)
        {
            var parser = new Parser(context);

            var sources = new List<(string ClassName, SourceText Source)>();

            //Containers
            var containersGenerator = new ContainersGenerator(context, parser);
            var containers = containersGenerator.GetContainers();
            sources.AddRange(containers);

            AddSourcesToContext(context, sources);
        }

        private void AddSourcesToContext(GeneratorExecutionContext context,
            List<(string ClassName, SourceText Source)> sources)
        {
            foreach (var source in sources)
            {
                context.AddSource(source.ClassName, source.Source);
            }
        }
    }
}
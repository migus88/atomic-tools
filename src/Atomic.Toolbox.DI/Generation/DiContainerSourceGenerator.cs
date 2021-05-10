using System;
using System.Collections.Generic;
using Atomic.Toolbox.Core.Parsers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

#if DEBUG
//#define START_DEBUG
#endif

namespace Atomic.Toolbox.DI.Generation
{
    [Generator]
    public class DiContainerSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG && START_DEBUG

            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
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
                    new DiagnosticDescriptor("ATDI", $"Container Generation didn't run",
                        "Container Generation didn't run. For more info, look for errors.", "Container Generation",
                        DiagnosticSeverity.Error, true),
                    null);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void HandleContext(GeneratorExecutionContext context)
        {
            var parser = new Parser(context.Compilation);

            //Containers
            var containersGenerator = new DiContainerHandler(parser);
            var containers = containersGenerator.GetContainers();

            AddSourcesToContext(context, containers);
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
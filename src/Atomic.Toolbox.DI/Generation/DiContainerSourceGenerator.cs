using System;
using System.Collections.Generic;
using Atomic.Toolbox.Core.Parsers;
using Atomic.Toolbox.DI.Generation.Models;
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
        
        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG && START_DEBUG

            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif
        }

        private void HandleContext(GeneratorExecutionContext context)
        {
            var parser = new Parser(context.Compilation);

            var containersGenerator = new DiContainerGenerationHandler(parser);
            containersGenerator.Generate();

            if (containersGenerator.HasSources)
            {
                AddSourcesToContext(context, containersGenerator.Sources);   
            }
        }

        private void AddSourcesToContext(GeneratorExecutionContext context,
            List<SourceModel> sources)
        {
            foreach (var source in sources)
            {
                context.AddSource(source.ClassName, source.Source);
            }
        }
    }
}
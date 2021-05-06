using System.Collections.Generic;
using Atomic.Generators.Tools.Parsers;
using Atomic.Injector.Generators.Analysis.Analyzers;
using Atomic.Injector.Generators.Analyzers;
using Atomic.Injector.Generators.Interfaces;

namespace Atomic.Injector.Generators.Analysis.Runners
{
    public class ContainerAnalysisRunner : AnalysisRunner
    {
        private readonly Parser _parser;

        protected override List<IAnalyzer> Analyzers => new List<IAnalyzer>
        {
            new DuplicateTransientInstallsAnalyzer(_parser),
        };

        public ContainerAnalysisRunner(Parser _parser)
        {
            this._parser = _parser;
        }
    }
}
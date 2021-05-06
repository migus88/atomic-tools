using System.Collections.Generic;
using Atomic.Injector.Generators.Analysis;
using Atomic.Injector.Generators.Interfaces;

namespace Atomic.Injector.Generators.Analyzers
{
    public abstract class AnalysisRunner
    {
        protected abstract List<IAnalyzer> Analyzers { get; }

        private readonly AnalysisReportList _reports = new AnalysisReportList();

        public AnalysisReportList Run()
        {
            foreach (var analyzer in Analyzers)
            {
                var report = analyzer.Analyze();
                _reports.AddRange(report);
            }

            return _reports;
        }
    }
}
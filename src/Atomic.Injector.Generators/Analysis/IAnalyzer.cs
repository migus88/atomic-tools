using System.Collections.Generic;
using Atomic.Injector.Generators.Analysis;

namespace Atomic.Injector.Generators.Interfaces
{
    public interface IAnalyzer
    {
        List<AnalysisReport> Analyze();
    }
}
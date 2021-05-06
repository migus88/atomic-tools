using System.Collections.Generic;
using System.Linq;

namespace Atomic.Injector.Generators.Analysis
{
    public class AnalysisReportList : List<AnalysisReport>
    {
        public bool HasFailedAnalysis()
        {
            return this.Count > 0 && this.Any(r => !r.IsPassed);
        }

        public List<AnalysisReport> GetFailedReports()
        {
            return !HasFailedAnalysis() ? null : this.Where(r => !r.IsPassed).ToList();
        }
    }
}
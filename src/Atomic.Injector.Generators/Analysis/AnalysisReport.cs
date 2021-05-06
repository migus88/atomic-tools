using Microsoft.CodeAnalysis;

namespace Atomic.Injector.Generators.Analysis
{
    public class AnalysisReport
    {
        public bool IsPassed { get; set; }
        public int AnalysisID { get; set; }
        public string Title { get; set; }
        public string ErrorMessage { get; set; }
        public Location HighlightData { get; set; }
    }
}
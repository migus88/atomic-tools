using System.Collections.Generic;
using Atomic.Generators.Tools.Parsers;
using Atomic.Injector.Generators.Interfaces;
using Microsoft.CodeAnalysis;

namespace Atomic.Injector.Generators.Analysis.Analyzers
{
    public abstract class BaseAnalyzer : IAnalyzer
    {
        public abstract int ID { get; }
        public abstract string Title { get; }
        public abstract string MessageTemplate { get; }

        protected readonly Parser _parser;

        public BaseAnalyzer(Parser parser)
        {
            _parser = parser;
        }

        public abstract List<AnalysisReport> Analyze();

        protected AnalysisReport CreateUnsuccessfulReport(Location location, params object[] arguments)
        {
            return new AnalysisReport
            {
                IsPassed = false,
                ErrorMessage = string.Format(MessageTemplate, arguments),
                HighlightData = location,
                AnalysisID = ID,
                Title = Title
            };
        }
    }
}
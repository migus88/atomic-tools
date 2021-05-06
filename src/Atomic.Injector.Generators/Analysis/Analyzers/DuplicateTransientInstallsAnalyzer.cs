using System.Collections.Generic;
using System.Linq;
using Atomic.Generators.Tools.Parsers;
using Microsoft.CodeAnalysis;
using static Atomic.Injector.Generators.Helpers.Identifiers.InstallAttributeTypes;

namespace Atomic.Injector.Generators.Analysis.Analyzers
{
    public class DuplicateTransientInstallsAnalyzer : BaseAnalyzer
    {
        public override int ID => AnalysisID.DuplicateTransientInstall;
        public override string Title => "Multiple Transient installs for the same type.";

        public override string MessageTemplate =>
            "{0} can't be registered as Transient more than once without a scope.";

        public DuplicateTransientInstallsAnalyzer(Parser parser) : base(parser)
        {
        }

        public override List<AnalysisReport> Analyze()
        {
            var trees = _parser.GetTreesWithInterface(ContainerInterfaceType);
            var reports = new AnalysisReportList();

            foreach (var tree in trees)
            {
                var classes = tree.GetClassesWithInterface(ContainerInterfaceType);

                foreach (var @class in classes)
                {
                    var fields = @class.GetFieldsWithAttributes(TransientAttributeType);

                    var newReports = fields
                        .GroupBy(f => f.TypeName)
                        .Select(g => g.ToList())
                        .Where(g => g.Count > 1)
                        .Select(g =>
                            CreateUnsuccessfulReport(
                                Location.Create(tree.SyntaxTree, g.First().Span),
                                g.First().TypeName))
                        .ToList();

                    reports.AddRange(newReports);
                }
            }

            return reports;
        }
    }
}
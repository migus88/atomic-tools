using Microsoft.CodeAnalysis.Text;

namespace Atomic.Toolbox.DI.Generation.Models
{
    public struct SourceModel
    {
        public string ClassName { get; set; }
        public SourceText Source { get; set; }
    }
}
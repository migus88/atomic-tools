using System.Collections.Generic;
using Atomic.Toolbox.DI.Generation.Models;

namespace Atomic.Toolbox.DI.Generation
{
    public interface IGenerationHandler
    {
        bool HasSources { get; }
        List<SourceModel> Sources { get; }
        
        void Generate();
        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexicalAnalyzer.Interfaces
{
    /// <summary>
    /// Interface for runtime objects that expose GUID identifiers to the web
    /// API.
    /// </summary>
    public interface IGuid
    {
        Guid Guid {
            get;
        }
    }
}

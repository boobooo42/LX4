using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LexicalAnalyzer.Interfaces
{
    public interface IStop
    {
        int downloadCount { get; }
        int downloadLimit { get; }
        Stopwatch timer { get; }
        int timeLimit { get; }

        bool stop();
    }
}

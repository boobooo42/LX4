using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LexicalAnalyzer.Interfaces
{
    public interface IStop
    {
        int DownloadCount { get; }
        int DownloadLimit { get; set; }
        Stopwatch Timer { get; }
        int TimeLimit { get; set; }

        bool downloadStop();
        bool timeStop();
    }
}

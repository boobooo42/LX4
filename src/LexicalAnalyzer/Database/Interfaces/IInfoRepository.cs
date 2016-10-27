using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexicalAnalyzer.Database
{
    interface IInfoRepository
    {
        int GetVersion();
    }
}

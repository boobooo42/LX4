using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexicalAnalyzer.Services
{
    public interface ITask
    {
        string Status
        {
            get;
        }

        /// <summary>
        /// This returns the progress of this task. The progress ranges from 0.0f
        /// to 1.0f.
        /// </summary>
        float Progress
        {
            get;
        }

        int Priority
        {
            get;
        }

        void Run();
    }
}

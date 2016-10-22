using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexicalAnalyzer.Interfaces
{
    /// <summary>
    /// Interface for processes that are to be run in background threads by
    /// Worker objects.
    /// </summary>
    public interface ITask
    {
        string Status
        {
            get;
            set;
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

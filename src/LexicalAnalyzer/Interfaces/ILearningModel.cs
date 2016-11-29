using LexicalAnalyzer.Models;
using LexicalAnalyzer.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace LexicalAnalyzer.Interfaces
{
    public interface ILearningModel : ITask, IGuid
    {
        string Type {
            get;
        }

        IEnumerable<KeyValueProperty> Properties
        {
            get; set;
        }
    }
}

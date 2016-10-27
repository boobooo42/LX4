using LexicalAnalyzer.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace LexicalAnalyzer.Interfaces
{
    public interface ILearningModel : ITask, IGuid
    {
        string DisplayName {
            get;
        }
        string Description {
            get;
        }
        string ContentType {
            get;
        }

        /*
        IEnumerable<KeyValueProperty> DefaultProperties {
            get;
        }
        */

        IEnumerable<KeyValueProperty> Properties {
            get;
        }
    }
}

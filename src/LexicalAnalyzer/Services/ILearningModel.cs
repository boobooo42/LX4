using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexicalAnalyzer.Services
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

using LexicalAnalyzer.Models;
using System.Collections.Generic;

namespace LexicalAnalyzer.Interfaces {
    public interface ILearningModelFactory {
        IEnumerable<LearningModelType> LearningModelTypes {
            get;
        }

        ILearningModel BuildLearningModel(string type);
    }
}

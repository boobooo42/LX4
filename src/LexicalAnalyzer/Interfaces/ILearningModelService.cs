using System.Collections.Generic;
using System;

namespace LexicalAnalyzer.Interfaces {
    public interface ILearningModelService {
        ILearningModel CreateLearningModel(string type);
        ILearningModel GetLearningModel(Guid guid);
        ILearningModel GetLearningModel(string guid);
        bool RemoveLearningModel(Guid guid);
        bool RemoveLearningModel(string guid);
        IEnumerable<ILearningModel> LearningModels { get; }
        void StartLearningModel(Guid guid);
        void StartLearningModel(string guid);
        void PauseLearningModel(Guid guid);
        void PauseLearningModel(string guid);
    }
}

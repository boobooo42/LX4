using LexicalAnalyzer.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace LexicalAnalyzer.Services {
    public class LearningModelService : ILearningModelService {
        /* Constants */
        private const int DEFAULT_NUM_WORKERS = 4;

        /* Private members */
        private List<ILearningModel> m_learningModels;
        private WorkerPool m_workerPool;
        private ILearningModelFactory m_factory;

        /* Constructors */
        public LearningModelService(ILearningModelFactory factory) {
            m_learningModels = new List<ILearningModel>();
            m_workerPool = new WorkerPool(DEFAULT_NUM_WORKERS);
            m_factory = factory;
        }

        /* Public interface */
        public ILearningModel CreateLearningModel(string type) {
            ILearningModel learningModel = m_factory.BuildLearningModel(type);
            if (learningModel == null) {
                return null;
            }
            Debug.Assert(
                    m_learningModels.Find(
                        elem => { return elem.Guid == learningModel.Guid; }
                        ) == null);
            m_learningModels.Add(learningModel);
            return learningModel;
        }

        public ILearningModel GetLearningModel(Guid guid) {
            return m_learningModels.Find(elem => { return elem.Guid == guid; });
        }
        public ILearningModel GetLearningModel(string guid) {
            return this.GetLearningModel(new System.Guid(guid));
        }

        public bool RemoveLearningModel(Guid guid) {
            ILearningModel learningModel = m_learningModels.Find(
                    elem => {
                        return elem.Guid == guid;
                    });
            if (learningModel == null) {
                return false;
            }
            /* TODO: Stop the learning model thread, if it exists */
            m_learningModels.Remove(learningModel);
            Debug.Assert(!m_learningModels.Contains(learningModel));
            return true;
        }
        public bool RemoveLearningModel(string guid) {
            return this.RemoveLearningModel(new System.Guid(guid));
        }

        public IEnumerable<ILearningModel> LearningModels {
            get {
                return m_learningModels;
            }
        }

        public void StartLearningModel(Guid guid) {
            ILearningModel learningModel = this.GetLearningModel(guid);
            if (learningModel == null)
                return;

            m_workerPool.StartTask(learningModel);
        }
        public void StartLearningModel(string guid) {
            this.StartLearningModel(new System.Guid(guid));
        }

        public void PauseLearningModel(Guid guid) {
            ILearningModel learningModel = this.GetLearningModel(guid);
            if (learningModel == null)
                return;

            /* TODO: Make sure the given learning task is paused */
            m_workerPool.PauseTask(learningModel);
        }
        public void PauseLearningModel(string guid) {
            this.PauseLearningModel(new System.Guid(guid));
        }
    }
}

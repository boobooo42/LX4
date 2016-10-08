using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexicalAnalyzer.Services
{
    public class WorkerPool
    {
        /* Singleton pattern */
        private static WorkerPool instance;

        private WorkerPool() {}

        private static WorkerPool Instance {
            get {
                if (instance == null) {
                    instance = new WorkerPool();
                }
                return instance;
            }
        } 

        /* Private members */
        private List<ITask> m_tasks;

        /* Accessors */
        private List<ITask> GetTasks() {
            return m_tasks;
        }
        public static List<ITask> Tasks {
            get {
                return Instance.GetTasks();
            }
        }
    }
}

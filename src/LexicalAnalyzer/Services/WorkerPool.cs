using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexicalAnalyzer.Services
{
    /* TODO: Somehow notify interested parties whenever a task finishes */
    public class WorkerPool
    {
        /* Private members */
        private List<ITask> m_tasks;
        private List<Worker> m_workers;

        public WorkerPool(int numWorkers) {
            for (int i = 0; i < numWorkers; ++i) {
                m_workers.Add(new Worker());
            }

            /* TODO: Start a thread for managing workers as they become idle */
        }

        /* Attributes */
        public List<ITask> Tasks {
            get {
                return m_tasks;
            }
        }

        /* Public methods */
        public void StartTask(ITask task) {
            /* TODO: Make sure this task is in our list of tasks? */
            /* TODO: Add this task to the task queue? */
            /* TODO: Set the status of this task to started? */
        }

        public void PauseTask(ITask task) {
            /* TODO: Find the worker that is running this task */
            /* TODO: Pause the thread that the worker is on (possibly unsafe?) */
        }
    }
}

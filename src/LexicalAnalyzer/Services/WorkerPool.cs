using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace LexicalAnalyzer.Services
{
    /* TODO: Somehow notify interested parties whenever a task finishes */
    public partial class WorkerPool
    {
        /* Private members */
        private List<ITask> m_tasks, m_readyTasks;
        private List<Worker> m_workers, m_readyWorkers;
        private Mutex m_workerMutex, m_readyMutex;
        private EventWaitHandle m_ready;
        private Thread m_dispatchThread;
        private volatile bool m_done;

        public WorkerPool(int numWorkers) {
            m_done = false;

            m_tasks = new List<ITask>();
            m_workers = new List<Worker>();
            for (int i = 0; i < numWorkers; ++i) {
                m_workers.Add(new Worker(this));
            }
            m_workerMutex = new Mutex();
            m_readyMutex = new Mutex();
            m_ready = new EventWaitHandle(false, EventResetMode.AutoReset);

            /* Start a thread for managing workers as they become ready */
            m_dispatchThread = new Thread(this.m_dispatch);
        }

        private void m_dispatch() {
            while (true) {
                /* TODO: Wait for workers to become ready */
                m_ready.WaitOne();
                if (m_done) {
                    return;
                }
                /* TODO: Check for pending tasks */
                m_readyMutex.ReleaseMutex();
            }
        }

        /* Attributes */
        public List<ITask> Tasks {
            get {
                return m_tasks;
            }
        }

        /* Public methods */
        public void StartTask(ITask task) {
            /* Make sure this task is in our list of tasks */
            if (!m_tasks.Contains(task)) {
                m_tasks.Add(task);
            }

            /* Make sure this task isn't already started */
            if (task.Status == "started") {
                return;
            }

            /* Check for available workers */
            m_workerMutex.WaitOne();
            Worker available = m_workers.Find(worker =>
                {
                    return worker.CurrentStatus == Worker.Status.READY;
                });
            if (available != null) {
            }
            m_workerMutex.ReleaseMutex();

            /* TODO: Add this task to the task queue? */
            /* TODO: Set the status of this task to started? */
            //task.Status = "queued";
        }

        public void PauseTask(ITask task) {
            /* TODO: Find the worker that is running this task */
            /* TODO: Pause the thread that the worker is on (possibly unsafe?) */
        }

        private void AddReadyWorker(Worker worker) {
            /* TODO: Make this method thread safe */
            m_readyMutex.WaitOne();
            Debug.Assert(m_workers.Contains(worker));
            Debug.Assert(!m_readyWorkers.Contains(worker));
            m_readyWorkers.Add(worker);
            m_readyMutex.ReleaseMutex();
            /* TODO: Notify the dispatch thread of a new worker */
            m_ready.Set();
        }
    }
}

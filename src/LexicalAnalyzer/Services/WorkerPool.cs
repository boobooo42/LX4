using LexicalAnalyzer.Interfaces;
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
        /* Workers */
        private List<Worker> m_workers, m_readyWorkers;
        private Mutex m_workersMutex, m_readyWorkersMutex;
        private EventWaitHandle m_readyWorkerEvent;
        /* Tasks */
        private List<ITask> m_tasks, m_readyTasks;
        private Mutex m_tasksMutex, m_readyTasksMutex;
        private EventWaitHandle m_readyTaskEvent;
        /* Dispatch thread */
        private Thread m_dispatchThread;
        private volatile bool m_done;

        public WorkerPool(int numWorkers) {
            m_done = false;

            /* Workers */
            m_workers = new List<Worker>();
            m_readyWorkers = new List<Worker>();
            m_workersMutex = new Mutex();
            m_readyWorkersMutex = new Mutex();
            m_readyWorkerEvent = new EventWaitHandle(
                    false,  /* initialState */
                    EventResetMode.AutoReset  /* mode */
                    );
            /* Tasks */
            m_tasks = new List<ITask>();
            m_readyTasks = new List<ITask>();
            m_tasksMutex = new Mutex();
            m_readyTasksMutex = new Mutex();
            m_readyTaskEvent = new EventWaitHandle(
                    false,  /* initialState */
                    EventResetMode.AutoReset  /* mode */
                    );

            /* Create our workers, starting each worker thread */
            for (int i = 0; i < numWorkers; ++i) {
                m_workers.Add(new Worker(this));
            }

            /* Start a thread for managing workers as they become ready */
            m_dispatchThread = new Thread(this.m_dispatch);
            m_dispatchThread.Start();
        }

        /// <summary>
        /// Method that operates the dispatch thread, deciding which tasks are
        /// assigned to which workers
        /// </summary>
        /// <remarks>
        /// <p>
        /// This method defines the behavior of the dispatch tread, which
        /// decides which tasks are assigned to which workers. It does this by
        /// waiting for a worker to become ready, and then waiting for a task
        /// to be queued.
        /// </p>
        /// <p>
        /// The dispatch thread is needed because most of the threads in a web
        /// application, such as the threads created to handle web API
        /// requests, are transient. In a traditional application, we would
        /// typically rely upon the main thread to dispatch our workers, but in
        /// a web application we do not have a main thread.
        /// </p>
        /// </remarks>
        private void m_dispatch() {
            /* TODO: End this thread gracefully */
            while (true) {
                /* Wait for a worker to become ready */
                m_readyWorkerEvent.WaitOne();
                m_readyWorkersMutex.WaitOne();
                /* Loop while we have workers */
                while (m_readyWorkers.Any()) {
                    /* NOTE: I think that we could probably wait for the
                     * m_readyTaskEvent here. The better thing to do, though,
                     * would be to reverse the outer loop and only wait for the
                     * m_readyWorkerEvent if we have nothing else to do. */
                    m_readyTasksMutex.WaitOne();
                    /* Loop while we have ready tasks and workers */
                    while (m_readyTasks.Any() && m_readyWorkers.Any()) {
                        /* Disptach workers to tasks */
                        m_readyWorkers.Last().Run(m_readyTasks.Last());
                        m_readyWorkers.RemoveAt(m_readyWorkers.Count - 1);
                        m_readyTasks.RemoveAt(m_readyTasks.Count - 1);
                    }
                    m_readyTasksMutex.ReleaseMutex();
                    if (m_readyWorkers.Any()) {
                        /* Wait for a task to become queued, since that
                         * is all we need at the moment */
                        m_readyTaskEvent.WaitOne();
                    }
                }
                m_readyWorkersMutex.ReleaseMutex();
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

            /* TODO: Simply add this task to the list of ready tasks; the
             * dispatch thread will handle the rest */
            m_readyTasksMutex.WaitOne();
            task.Status = "started";  /* FIXME: I need better status definitions */
            Debug.Assert(!m_readyTasks.Contains(task));
            m_readyTasks.Add(task);
            m_readyTasksMutex.ReleaseMutex();
            /* Signal the dispatch thread */
            m_readyTaskEvent.Set();
        }

        public void PauseTask(ITask task) {
            /* TODO: Find the worker that is running this task */
            /* TODO: Pause the thread that the worker is on (possibly unsafe?) */
            /* TODO: Simply add this task to the list of ready tasks; the
             * dispatch thread will handle the rest */
            m_readyTasksMutex.WaitOne();
            task.Status = "paused";  /* FIXME: I need better status definitions */
            Debug.Assert(!m_readyTasks.Contains(task));
            m_readyTasks.Remove(task);
            m_readyTasksMutex.ReleaseMutex();
            /* Signal the dispatch thread */
            //m_readyTaskEvent.Dispose();
        }

        private void AddReadyWorker(Worker worker)
        {
            m_readyWorkersMutex.WaitOne();
            Debug.Assert(!m_readyWorkers.Contains(worker));
            m_readyWorkers.Add(worker);
            m_readyWorkersMutex.ReleaseMutex();
            /* Notify the dispatch thread of a new worker */
            m_readyWorkerEvent.Set();
        }
    }
}

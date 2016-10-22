using LexicalAnalyzer.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace LexicalAnalyzer.Services
{
    public partial class WorkerPool {
        public class Worker
        {
            public enum Status : int {
                INIT,
                READY,
                BUSY,
                PAUSED,
            };

            /* Private members */
            private Status m_status;
            private Thread m_workerThread;
            private ITask m_task;
            private Mutex m_taskMutex;
            private WorkerPool m_pool;
            private EventWaitHandle m_taskReady;
            private volatile bool m_join;

            public Worker(WorkerPool pool) {
                m_status = Status.INIT;
                m_pool = pool;

                m_taskMutex = new Mutex();
                m_taskReady = new EventWaitHandle(
                        false,  /* initialState */
                        EventResetMode.AutoReset  /* mode */
                        );

                /* TODO: Start the thread and get it ready */
                m_workerThread = new Thread(this.m_Run);
                m_workerThread.Start();
            }

            public Status CurrentStatus {
                get {
                    return m_status;
                }
            }

            /* Public methods */
            public void Run(ITask task) {
                /* Set the task to be run */
                m_taskMutex.WaitOne();
                Debug.Assert(m_task == null);
                m_task = task;
                m_taskMutex.ReleaseMutex();
                /* Notify the worker thread of its task */
                m_taskReady.Set();
            }

            /* Private methods */
            void m_Run() {
                while (true) { 
                    /* Notify the dispatch thread that we are ready */
                    m_pool.AddReadyWorker(this);
                    /* Wait for our task to be assigned */
                    m_taskReady.WaitOne();
                    if (m_join) {
                        /* Gracefully exit early */
                        return;
                    }
                    Debug.Assert(m_task != null);
                    /* Actually run the task */
                    m_task.Run();
                    m_task = null;
                }
            }
        }
    }
}

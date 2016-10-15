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
                m_taskReady = new EventWaitHandle(true, EventResetMode.AutoReset);

                /* TODO: Start the thread and get it ready */
            }

            public Status CurrentStatus {
                get {
                    return m_status;
                }
            }

            /* Public methods */
            void Run(ITask task) {
                /* Set the task to be run */
                m_taskMutex.WaitOne();
                Debug.Assert(m_task == null);
                m_task = task;
                m_taskMutex.ReleaseMutex();

                /* TODO: Wake up the worker thread */
            }

            /* Private methods */
            void m_Run() {
                while (true) { 
                    /* TODO: Lock the task mutex early to avoid racing the dispatch
                     * thread */
                    m_taskMutex.WaitOne();
                    /* TODO: Indicate t */
                    m_pool.AddReadyWorker(this);
                    /* TODO: Wait for a task to be assigned to this worker */
                    if ((m_task == null) && !m_join) {
                        m_taskReady.WaitOne();
                    }
                }
            }
        }
    }
}

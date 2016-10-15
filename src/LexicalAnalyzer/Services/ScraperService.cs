using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace LexicalAnalyzer.Services
{
    public class ScraperService
    {
        /* Constants */
        private const int DEFAULT_NUM_WORKERS = 4;

        /* Singleton pattern */
        private static ScraperService m_instance;
        private static ScraperService Instance {
            get {
                if (m_instance == null) {
                    m_instance = new ScraperService();
                }
                return m_instance;
            }
        }

        private ScraperService() {
            m_scrapers = new List<IScraper>();
            m_workerPool = new WorkerPool(DEFAULT_NUM_WORKERS);
        }

        /* Private members */
        private List<IScraper> m_scrapers;
        private WorkerPool m_workerPool;

        /* Public interface */
        private IScraper m_CreateScraper(string type) {
            IScraper scraper = ScraperFactory.BuildScraper(type);
            if (scraper == null) {
                return null;
            }
            Debug.Assert(
                    m_scrapers.Find(
                        elem => { return elem.Guid == scraper.Guid; }
                        ) == null);
            m_scrapers.Add(scraper);
            return scraper;
        }
        public static IScraper CreateScraper(string type) {
            return Instance.m_CreateScraper(type);
        }

        private IScraper m_GetScraper(Guid guid) {
            return m_scrapers.Find(elem => { return elem.Guid == guid; });
        }
        public static IScraper GetScraper(Guid guid) {
            return Instance.m_GetScraper(guid);
        }
        public static IScraper GetScraper(string guid) {
            return Instance.m_GetScraper(new System.Guid(guid));
        }

        private bool m_RemoveScraper(Guid guid) {
            IScraper scraper = m_scrapers.Find(
                    elem => {
                        return elem.Guid == guid;
                    });
            if (scraper == null) {
                return false;
            }
            /* TODO: Stop the scraping thread, if it exists */
            m_scrapers.Remove(scraper);
            Debug.Assert(!m_scrapers.Contains(scraper));
            return true;
        }
        public static bool RemoveScraper(Guid guid) {
            return Instance.m_RemoveScraper(guid);
        }
        public static bool RemoveScraper(string guid) {
            return Instance.m_RemoveScraper(new System.Guid(guid));
        }

        private List<IScraper> m_Scrapers {
            get {
                return m_scrapers;
            }
        }
        public static List<IScraper> Scrapers {
            get {
                return Instance.m_Scrapers;
            }
        }

        private void m_StartScraper(Guid guid) {
            IScraper scraper = m_GetScraper(guid);
            if (scraper == null)
                return;

            m_workerPool.StartTask(scraper);
        }
        public static void StartScraper(Guid guid) {
            Instance.m_StartScraper(guid);
        }
        public static void StartScraper(string guid) {
            Instance.m_StartScraper(new System.Guid(guid));
        }

        private void m_PauseScraper(Guid guid) {
            IScraper scraper = m_GetScraper(guid);
            if (scraper == null)
                return;

            /* TODO: Make sure the given scraper task is paused */
            m_workerPool.PauseTask(scraper);
        }
        public static void PauseScraper(Guid guid) {
            Instance.m_PauseScraper(guid);
        }
        public static void PauseScraper(string guid) {
            Instance.m_PauseScraper(new System.Guid(guid));
        }
    }
}

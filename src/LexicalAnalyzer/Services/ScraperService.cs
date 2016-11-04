using LexicalAnalyzer.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace LexicalAnalyzer.Services
{
    public class ScraperService : IScraperService
    {
        /* Constants */
        private const int DEFAULT_NUM_WORKERS = 4;

        /* Private members */
        private List<IScraper> m_scrapers;
        private WorkerPool m_workerPool;
        private IScraperFactory m_scraperFactory;

        public ScraperService(IScraperFactory scraperFactory) {
            m_scrapers = new List<IScraper>();
            m_workerPool = new WorkerPool(DEFAULT_NUM_WORKERS);
            m_scraperFactory = scraperFactory;
        }

        /* Public interface */
        public IScraper CreateScraper(string type) {
            IScraper scraper = m_scraperFactory.BuildScraper(type);
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

        public IScraper GetScraper(Guid guid) {
            return m_scrapers.Find(elem => { return elem.Guid == guid; });
        }
        public IScraper GetScraper(string guid) {
            return this.GetScraper(new System.Guid(guid));
        }

        public bool RemoveScraper(Guid guid) {
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
        public bool RemoveScraper(string guid) {
            return this.RemoveScraper(new System.Guid(guid));
        }

        public IEnumerable<IScraper> Scrapers {
            get {
                return m_scrapers;
            }
        }

        public void StartScraper(Guid guid) {
            IScraper scraper = this.GetScraper(guid);
            if (scraper == null)
                return;

            m_workerPool.StartTask(scraper);
        }
        public void StartScraper(string guid) {
            this.StartScraper(new System.Guid(guid));
        }

        public void PauseScraper(Guid guid) {
            IScraper scraper = this.GetScraper(guid);
            if (scraper == null)
                return;

            /* TODO: Make sure the given scraper task is paused */
            m_workerPool.PauseTask(scraper);
        }
        public void PauseScraper(string guid) {
            this.PauseScraper(new System.Guid(guid));
        }
    }
}

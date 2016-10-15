using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexicalAnalyzer.Services
{
    public class TestScraper : IScraper
    {
        /* Private members */
        private Guid m_guid;
        private string m_status;

        TestScraper() {
            m_guid = System.Guid.NewGuid();
            m_status = "init";
        }

        /* Public Interface */
        public Guid Guid {
            get {
                return m_guid;
            }
        }

        public string DisplayName {
            get { return "Test Scraper"; }
        }

        public string Description {
            get {
                return
                    @"A scraper used for testing purposes. This scraper does
                    not actually scrape any content; it simply pretends to
                    scrape content.";
            }
        }

        public string ContentType {
            get {
                return "text";
            }
        }

        public string Status {
            get {
                return m_status;
            }
            set {
                m_status = value;
            }
        }

        public float Progress {
            get {
                return 0.0f;
            }
        }

        public int Priority {
            get {
                return 0;
            }
        }

        public void Run() {
            /* TODO: Implement a fake scraper that simply waits for a few
             * minutes and periodically increments the progress */
        }
    }
}

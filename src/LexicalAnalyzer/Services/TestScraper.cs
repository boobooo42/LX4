using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace LexicalAnalyzer.Services
{
    public class TestScraper : IScraper
    {
        /* Private members */
        private Guid m_guid;
        private string m_status;
        private float m_progress;
        private int m_priority;
        private List<KeyValueProperty> m_properties;

        public TestScraper() {
            m_guid = System.Guid.NewGuid();
            m_status = "init";
            m_progress = 0.0f;
            m_priority = 0;
            m_properties = new List<KeyValueProperty>();
            m_properties.Add(
                    new KeyValueProperty(
                        "timeout",  /* key */
                        "30",  /* defaultValue */
                        "seconds"  /* type */
                        ));
            m_properties.Add(
                    new KeyValueProperty(
                        "website",  /* key */
                        "http://example.com",  /* defaultValue */
                        "url"  /* type */
                        ));
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
                return m_progress;
            }
        }

        public int Priority {
            get {
                return m_priority;
            }
        }

        public IEnumerable<KeyValueProperty> Properties {
            get {
                return m_properties;
            }
        }

        public void Run() {
            /* Implement a fake scraper that simply waits for a while and
             * periodically increments the progress */
            while (m_progress < 1.0f) {
                Thread.Sleep(5000);
                m_progress += 0.1f;
            }
        }
    }
}

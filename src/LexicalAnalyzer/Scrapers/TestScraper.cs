using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using LexicalAnalyzer.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Diagnostics;

namespace LexicalAnalyzer.Scrapers
{
    public class TestScraper : IScraper
    {
        /* Private members */
        private Guid m_guid;
        private string m_status;
        private float m_progress;
        private int m_priority;
        private ICorpusContext m_context;
        private int m_downloadCount;
        private int m_downloadLimit;
        private Stopwatch m_timer;
        private int m_timeLimit;


        public TestScraper(ICorpusContext context) {
            m_guid = System.Guid.NewGuid();
            m_status = "init";
            m_progress = 0.0f;
            m_priority = 0;
            m_context = context;
            m_downloadCount = 0;
            m_downloadLimit = 0;
            m_timer = new Stopwatch();
            m_timeLimit = 0;
        }

        /* Public Interface */
        public Guid Guid {
            get {
                return m_guid;
            }
        }

        public static string DisplayName {
            get { return "Test Scraper"; }
        }

        public static string Description {
            get {
                return
                    @"A scraper used for testing purposes. This scraper does
                    not actually scrape any content; it simply pretends to
                    scrape content.";
            }
        }

        public static string ContentType {
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

        public int downloadCount
        {
            get
            {
                return m_downloadCount;
            }
            set
            {
                m_downloadCount = value;
            }
        }

        public int downloadLimit
        {
            get
            {
                return m_downloadLimit;
            }
            set
            {
                m_downloadLimit = value;
            }
        }

        public Stopwatch timer
        {
            get
            {
                return m_timer;
            }
        }

        public int timeLimit
        {
            get
            {
                return m_timeLimit;
            }
            set
            {
                m_timeLimit = value;
            }
        }

        public static IEnumerable<KeyValueProperty> DefaultProperties {
            get {
                var properties = new List<KeyValueProperty>();
                properties.Add(
                        new KeyValueProperty(
                            "timeout",  /* key */
                            "30",  /* defaultValue */
                            "seconds"  /* type */
                            ));
                properties.Add(
                        new KeyValueProperty(
                            "website",  /* key */
                            "http://example.com",  /* defaultValue */
                            "url"  /* type */
                            ));
                return new List<KeyValueProperty>();
            }
        }

        public IEnumerable<KeyValueProperty> Properties {
            get; set;
        }

        public void Run() {
            timer.Reset();
            timer.Start();
            /* Implement a fake scraper that simply waits for a while and
             * periodically increments the progress */
            while (!stop()) {
                Thread.Sleep(5000);
                downloadCount++;
                m_progress = downloadCount / downloadLimit;
            }
        }

        public bool stop()
        {
            if (downloadCount >= downloadLimit || timer.ElapsedMilliseconds >= timeLimit * 1000)
                return true;
            else
                return false;
        }
    }
}

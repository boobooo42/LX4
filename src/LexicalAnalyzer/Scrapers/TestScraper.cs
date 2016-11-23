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
        private List<KeyValueProperty> m_properties;


        public TestScraper(ICorpusContext context) {
            m_guid = System.Guid.NewGuid();
            m_status = "init";
            m_progress = 0.0f;
            m_priority = 0;
            m_context = context;
            m_downloadCount = 0;
            m_downloadLimit = 2;
            m_timer = new Stopwatch();
            m_timeLimit = 3;
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

        public int DownloadCount
        {
            get
            {
                return m_downloadCount;
            }
        }
        public int DownloadLimit
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

        public Stopwatch Timer
        {
            get
            {
                return m_timer;
            }
        }

        public int TimeLimit
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
            get
            {
                var properties = new List<KeyValueProperty>();
                properties.Add(
                        new KeyValueProperty(
                            "timelimit",  /* key */
                            "30",  /* defaultValue */
                            "seconds"  /* type */
                            ));
                properties.Add(
                        new KeyValueProperty(
                            "downloadlimit",  /* key */
                            "30",  /* defaultValue */
                            "items"  /* type */
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
            get { return m_properties;  }
            set
            {
                foreach (var property in value)
                {
                    if (property.Key == "timeLimit")
                        TimeLimit = int.Parse(property.Value);
                    else if (property.Key == "downloadlimit")
                        DownloadLimit = int.Parse(property.Value);
                }
                m_properties = new List<KeyValueProperty>(value);
            }
        }


        public void Run() {
            /* Implement a fake scraper that simply waits for a while and
 * periodically increments the progress */
            m_downloadCount = 0;
            m_timer.Reset();            
            bool downloadLimitReached = downloadStop();
            bool timeLimitReached = timeStop();
            m_timer.Start();
            while (!downloadLimitReached && !timeLimitReached) {
                Thread.Sleep(5000);
                m_downloadCount++;
                m_progress = m_downloadCount / m_downloadLimit;
                downloadLimitReached = downloadStop();
                timeLimitReached = timeStop();
            }
            m_status = "stopped on ";
            if(downloadLimitReached && timeLimitReached)
                m_status += "downloads, time";
            else if (downloadLimitReached)
                m_status += "downloads";
            else if(timeLimitReached)
                m_status += "time";
        }

        public bool downloadStop()
        {
            if (DownloadCount >= DownloadLimit)
                return true;
            else
                return false;
        }

        public bool timeStop()
        {
            if (m_timer.ElapsedMilliseconds >= TimeLimit * 1000)
            {
                m_timer.Reset();
                return true;
            }
            else
                return false;
        }
    }
}

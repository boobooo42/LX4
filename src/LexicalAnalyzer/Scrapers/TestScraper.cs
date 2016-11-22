﻿using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using LexicalAnalyzer.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;

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

        public TestScraper(ICorpusContext context) {
            m_guid = System.Guid.NewGuid();
            m_status = "init";
            m_progress = 0.0f;
            m_priority = 0;
            m_context = context;
        }

        /* Public Interface */
        /// <summary>
        /// Gets the scraper type
        /// </summary>
        /// <returns></returns>
        public string Type
        {
            get
            {
                return this.GetType().FullName;
            }
        }
        public Guid Guid {
            get {
                return m_guid;
            }
        }

        public static string DisplayName {
            get { return "Test Scraper"; }
        }
        public string DName { get { return "Test Scraper"; } }

        public static string Description {
            get {
                return
                    @"A scraper used for testing purposes. This scraper does
                    not actually scrape any content; it simply pretends to
                    scrape content.";
            }
        }
        public string Desc
        {
            get { return @"A scraper used for testing purposes. This scraper does
                    not actually scrape any content; it simply pretends to
                    scrape content."; }
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
            /* Implement a fake scraper that simply waits for a while and
             * periodically increments the progress */
            while (m_progress < 1.0f) {
                Thread.Sleep(5000);
                m_progress += 0.1f;
            }
        }
    }
}

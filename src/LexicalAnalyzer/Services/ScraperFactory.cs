﻿using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace LexicalAnalyzer.Services
{
    public class ScraperFactory
    {
        /* Singleton pattern */
        private static ScraperFactory m_instance;
        private static ScraperFactory Instance {
            get {
                if (m_instance == null) {
                    m_instance = new ScraperFactory();
                }
                return m_instance;
            }
        }

        private List<Type> m_scraperTypes;

        private ScraperFactory() {
            m_scraperTypes = new List<Type>();

            /* Fill our array of scraper types */
            m_scraperTypes.Add(typeof(DebianScraper));

            /* Ensure that each scraper type implements IScraper */
            foreach (Type t in m_scraperTypes) {
                Debug.Assert(t.GetInterfaces().Contains(typeof(IScraper)));
            }
        }

        private List<string> m_ScraperNames {
            get {
                List<string> result = new List<string>();

                foreach (Type t in m_scraperTypes) {
                    result.Add(t.FullName);
                }

                return result;
            }
        }
        public static List<string> ScraperNames {
            get {
                return Instance.m_ScraperNames;
            }
        }

        private IScraper m_BuildScraper(string name) {
            Type t = m_scraperTypes.Find(elem => { return elem.Name == name; });
            if (t.IsAssignableFrom(typeof(IScraper))) {
                return (IScraper)Activator.CreateInstance(t);
            }
            return null;
        }
        public static IScraper BuildScraper(string name) {
            return Instance.m_BuildScraper(name);
        }
    }
}

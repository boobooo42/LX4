using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Scrapers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System;

namespace LexicalAnalyzer.Services
{
    public class ScraperFactory
    {
        /* Singleton pattern */
        /* FIXME: Replace this singleton pattern with dependency injection */
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
            m_scraperTypes.Add(typeof(TestScraper));
            m_scraperTypes.Add(typeof(TextScraper));

            /* TODO: Add scrapers from DLL assemblies */

            /* Ensure that each scraper type implements IScraper */
            foreach (Type t in m_scraperTypes) {
                Debug.Assert(t.GetInterfaces().Contains(typeof(IScraper)));
            }
        }

        private List<string> m_ScraperTypes {
            get {
                List<string> result = new List<string>();

                foreach (Type t in m_scraperTypes) {
                    result.Add(t.FullName);
                }

                return result;
            }
        }
        public static List<string> ScraperTypes {
            get {
                return Instance.m_ScraperTypes;
            }
        }

        private IScraper m_BuildScraper(string type) {
            Type t = m_scraperTypes.Find(elem => { return elem.FullName == type; });
            if (t == null) {
                return null;
            }
            if (t.GetInterfaces().Contains(typeof(IScraper))) {
                return (IScraper)Activator.CreateInstance(t);
            }
            return null;
        }
        public static IScraper BuildScraper(string type) {
            return Instance.m_BuildScraper(type);
        }
    }
}

using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using LexicalAnalyzer.Scrapers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System;

namespace LexicalAnalyzer.Services
{
    public class ScraperFactory : IScraperFactory
    {
        /* Private members */
        private List<Type> m_scraperTypes;
        private ICorpusContext m_context;

        /* Constructors */
        public ScraperFactory(ICorpusContext context) {
            m_context = context;
            m_scraperTypes = new List<Type>();

            /* Fill our array of scraper types */
            m_scraperTypes.Add(typeof(DebianScraper));
            m_scraperTypes.Add(typeof(TestScraper));
            m_scraperTypes.Add(typeof(TextScraper));
            m_scraperTypes.Add(typeof(TwitterScraper));
            m_scraperTypes.Add(typeof(GithubScraper));

            /* TODO: Add scrapers from DLL assemblies */

            /* Ensure that each scraper type implements IScraper */
            foreach (Type t in m_scraperTypes) {
                Debug.Assert(t.GetInterfaces().Contains(typeof(IScraper)));
            }

            /* TODO: Ensure that each scraper type implements the needed
             * static methods (with appropriate signatures) */
        }

        public IEnumerable<ScraperType> ScraperTypes {
            get {
                List<ScraperType> result = new List<ScraperType>();

                foreach (Type t in m_scraperTypes) {
                    ScraperType st = new ScraperType();
                    /* Store the fully qualified name of the implementing
                     * class */
                    st.Type = t.FullName;
                    /* Invoke the respective static methods for this type */
                    st.DisplayName = (string)t
                        .GetProperty("DisplayName",
                                BindingFlags.Public | BindingFlags.Static)
                        .GetValue(null, null);
                    st.Description = (string)t
                        .GetProperty("Description",
                                BindingFlags.Public | BindingFlags.Static)
                        .GetValue(null);
                    st.ContentType = (string)t
                        .GetProperty("ContentType",
                                BindingFlags.Public | BindingFlags.Static)
                        .GetValue(null);
                    st.Properties = (IEnumerable<KeyValueProperty>)
                        t.GetProperty("DefaultProperties",
                                BindingFlags.Public | BindingFlags.Static)
                        .GetValue(null);
                    result.Add(st);
                }

                return result;
            }
        }

        public IScraper BuildScraper(string type) {
            Type t = m_scraperTypes.Find(elem => { return elem.FullName == type; });
            if (t == null)
                return null;
            if (!t.GetInterfaces().Contains(typeof(IScraper)))
                return null;
            object[] arguments = new object[] { m_context };
            return (IScraper)Activator.CreateInstance(t, arguments);
        }
    }
}

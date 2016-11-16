using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using RestEase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LexicalAnalyzer.Scrapers
{
    public class GithubScraper : IScraper
    {
        private Guid m_guid;
        private string m_status;
        private float m_progress;
        private int m_priority;
        private ICorpusContext m_context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <returns></returns>
        public GithubScraper(ICorpusContext context)
        {
            m_guid = System.Guid.NewGuid();
            m_status = "init";
            m_progress = 0.0f;
            m_priority = 0;
            m_context = context;
        }

        #region Properties
        /// <summary>
        /// Gets guid
        /// </summary>
        /// <returns></returns>
        public Guid Guid
        {
            get
            {
                return m_guid;
            }
        }

        /// <summary>
        /// Gets display name--is hardcoded
        /// </summary>
        /// <returns></returns>
        public static string DisplayName
        {
            get { return "Github Scraper"; }
        }

        /// <summary>
        /// Gets description--is hardcoded
        /// </summary>
        /// <returns></returns>
        public static string Description
        {
            get
            {
                return
                    @"A scraper used for scraping repositories from Github.";
            }
        }

        /// <summary>
        /// Gets content type --is hardcoded
        /// </summary>
        /// <returns></returns>
        public static string ContentType
        {
            get
            {
                return "Repositories";
            }
        }

        /// <summary>
        /// Gets status
        /// </summary>
        /// <returns></returns>
        public string Status
        {
            get
            {
                return m_status;
            }
            set
            {
                m_status = value;
            }
        }

        /// <summary>
        /// Gets progress
        /// </summary>
        /// <returns></returns>
        public float Progress
        {
            get
            {
                return m_progress;
            }
        }

        /// <summary>
        /// Gets priority
        /// </summary>
        /// <returns></returns>
        public int Priority
        {
            get
            {
                return m_priority;
            }
        }

        /// <summary>
        /// List of properties supported by TextScraper and their respective
        /// default values.
        /// </summary>
        public static IEnumerable<KeyValueProperty> DefaultProperties
        {
            get
            {
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
                            "https://github.com",  /* defaultValue */
                            "url"  /* type */
                            ));
                properties.Add(
                        new KeyValueProperty(
                            "tweetsToDownload", /* key */
                            "", /* defaultValue */
                            "urls" /* type */
                            ));
                return properties;
            }
        }

        /// <summary>
        /// Gets properties
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValueProperty> Properties
        {
            get; set;
        }
        #endregion

        /// <summary>
        /// Runs the github scraper
        /// </summary>
        /// <returns></returns>
        public void Run()
        {
            Debug.Assert(false);
            ScrapeGithub();
        }

        private async void ScrapeGithub()
        {
            // Create an implementation of that interface
            // We'll pass in the base URL for the API
            IGitHubApi api = RestClient.For<IGitHubApi>("https://api.github.com");

            // Now we can simply call methods on it
            // Sends a GET request to https://api.github.com
            List<Repos> repositories = await api.GetReposAsync();
        }
    }
}

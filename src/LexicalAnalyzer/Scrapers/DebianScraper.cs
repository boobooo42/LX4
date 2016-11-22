using HtmlAgilityPack;
using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using LexicalAnalyzer.Resources;
using LexicalAnalyzer.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;

namespace LexicalAnalyzer.Scrapers
{
    /// <summary>
    /// Scraper for the Debian website
    /// </summary>
    public class DebianScraper : IScraper
    {
        public DebianScraper(ICorpusContext context) {
            m_guid = System.Guid.NewGuid();
            m_status = "init";
            m_context = context;
        }

        #region Structs
        struct splitUrl
        {
            public string baseUrl;
            public string file;

            public splitUrl(string _baseUrl, string _file)
            {
                baseUrl = _baseUrl;
                file = _file;
            }
        }
        #endregion

        #region Globals
        /* Constants */
        private const string rootURL = "http://debian.osuosl.org/debian/pool/main/c/";
        private const string DownloadPath = "//a[contains(@href,'.deb')]";
        private const string LinkPath = "//a[@href]";

        /* Internal Members */
        private string exeLinks;
        private List<string> urlList;
        private HtmlDocumentTree htmlDocumentTree;
        List<HtmlNode> DownLinkList;
        HtmlNodeCollection DownCollection;
        private Guid m_guid;
        private ICorpusContext m_context;
        #endregion

        #region  Public Interface
        /// <summary>
        /// Gets a new guid
        /// </summary>
        /// <returns></returns>
        public Guid Guid {
            get {
                return m_guid;
            }
        }

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

        /// <summary>
        /// Gets the display name
        /// </summary>
        /// <returns></returns>
        public static string DisplayName {
            get { return "Debian Scraper"; }
        }

        /// <summary>
        /// Gets the description
        /// </summary>
        /// <returns></returns>
        public static string Description {
            get {
                return
                    @"Useful for scraping .deb files from the Debian archive
                    mirrors. Files can be scraped for a variety of different
                    architectures, including x86 and x86_64.";
            }
        }

        /// <summary>
        /// Gets the content type
        /// </summary>
        /// <returns></returns>
        public static string ContentType {
            get {
                return "executables";
            }
        }

        private string m_status;
        /// <summary>
        /// Gets the status
        /// </summary>
        /// <returns></returns>
        public string Status {
            get {
                return m_status;
            }
            set {
                m_status = value;
            }
        }

        /// <summary>
        /// Gets the progress
        /// </summary>
        /// <returns></returns>
        public float Progress
        {
            get
            {
                return 0.0f;
            }
        }

        public int Priority {
            get {
                return 0;
            }
        }

        public static IEnumerable<KeyValueProperty> DefaultProperties {
            get {
                var properties = new List<KeyValueProperty>();
                return properties;
            }
        }

        public IEnumerable<KeyValueProperty> Properties {
            get; set;
        }
        #endregion

        #region Run
        /// <summary>
        /// Runs the scraper
        /// </summary>
        /// <returns></returns>
        public void Run() {
            Debug.Assert(false);  /* XXX */
            /* TODO: Perform scraping here */
            /* TODO: We do not have a model for corpus content yet, so there is
             * nowhere to put anything we download yet */

            //  string debs = GetBinaries(myURL);

            List<string> downLoadTypes = new List<string>();
            downLoadTypes.Add(".deb");
            downLoadTypes.Add(".tar.gz");

            string debs = "";
            
            //work being done here
            //downLoadTypes = GetDownloads(downLoadTypes, urlList);

            foreach (string s in downLoadTypes)
            {
                debs += s + "\n";
            }
        }

        /// <summary>
        /// new method to clean up get method in controller
        /// </summary>
        /// <returns></returns>
        public string RunDisplay()
        {
            //   string baseUrl = "http://debian.osuosl.org/debian/pool/main/c/";
            Task<string> task = ScraperUtilities.AsyncUrlToTask(rootURL);
            task.Wait();

            var testDoc = new HtmlDocument();
            testDoc.LoadHtml(task.Result);
            htmlDocumentTree = ScraperUtilities.createHtmlDocTree(testDoc, rootURL, LinkPath, rootURL);
            //List<string> htmlDocumentHashes = new List<string>();
            //htmlDocumentHashes.Add(getHash(testDoc));
            //HtmlDocumentTree tree = createHtmlDocTree(new HtmlDocumentTree(testDoc, rootURL), htmlDocumentHashes);
            string result = "";
            //result += tree.ChildDocuments.Count.ToString() + "\n";
            result += ScraperUtilities.displayHtmlDocumentTree(htmlDocumentTree);
            // result = displayAllUrls(testDoc);
            //result += displayInnerUrls(testDoc);
            //result += getFiles(testDoc);

            result = "";

            //this just lists all the urls
            foreach (var x in urlList)
            {
                result += x + "\n";
            }

            return result;
        }
        #endregion
    }
}

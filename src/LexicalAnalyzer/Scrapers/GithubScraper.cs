using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using RestEase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
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
                            "gitsToScrape", /* key */
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
            ScrapeGitHub();
        }

        /// <summary>
        /// wrapper for GitHubScraper
        /// </summary>
        private void ScrapeGitHub()
        {
            //  string x = ScrapeRepo().Result;
            Uri uri = new Uri("https://api.github.com/repos/LunarG/VulkanTools");
            var tags = GetRepoTags(uri).Result;
            Uri tagURI = new Uri(tags[0].zipball_url);
            var byteA = ExtractFromUrl(tagURI).Result;
            var decompedA = Decompress(byteA);
        }

        private async Task<string> ScrapeRepo()
        {
            Uri gitHubUri = new Uri("https://api.github.com");
            // Create an implementation of that interface
            // We'll pass in the base URL for the API
            IGitHubApi api = RestClient.For<IGitHubApi>(gitHubUri.AbsoluteUri);

            // Sends a GET request to https://api.github.com
            var repositories = await api.GetReposAsync();

            //get the response header
            var headers = repositories.ResponseMessage.Headers;
            string nextRepoList = "";
            IEnumerable<string> headerValues;
            if (headers.TryGetValues("link", out headerValues)) //find the link field in response header
            {
                nextRepoList = headerValues.First();
            }

            //here we are getting the link to the next page using  a regex split
            string nextPageLink = "";
            foreach (string pair in nextRepoList.Split(','))
            {
                var splitPair = pair.Split(';');
                var re = new Regex("<(.*)>");
                string URI = re.Match(splitPair[0]).Groups[1].Value;
                if (splitPair[1].Contains("next"))
                    nextPageLink = URI;
            }

            //getting a list of tags from each repository
            List<Repos> repoList = repositories.GetContent();
            List<string> tagList = new List<string>();
            foreach (Repos repo in repoList)
            {
                tagList.Add(repo.url);
                Uri tagUri = new Uri(repo.url);
                var tags = await GetRepoTags(tagUri);
            }


            return nextPageLink;
        }

        private async Task<List<Tags>> GetRepoTags(Uri repoTagUrl)
        {
            // Create an implementation of that interface
            // We'll pass in the base URL for the API
            IGitHubApi api = RestClient.For<IGitHubApi>(repoTagUrl.AbsoluteUri);

            // Now we can simply call methods on it
            var response = await api.GetTagsAsync();

            if (response.ResponseMessage.IsSuccessStatusCode)
                return response.GetContent();
            else return null;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private async Task<byte[]> ExtractFromUrl(Uri uri)
        {

            //using (HttpClient client = new HttpClient())
            //{
            //    //var y = await client.GetByteArrayAsync(uri);

            //    client.BaseAddress = uri;
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/zip"));
            //    var response = await client.GetAsync(uri);
            //    if (response.IsSuccessStatusCode)
            //        return response.Content.ReadAsByteArrayAsync().Result;
            //    else return null;
            //}
            // Create an implementation of that interface
            // We'll pass in the base URL for the API
            IGitHubApi api = RestClient.For<IGitHubApi>(uri.AbsoluteUri);

            // Now we can simply call methods on it
            var response = await api.GetByteAsync();

            if (response.ResponseMessage.IsSuccessStatusCode)
                return response.GetContent();
            else return null;

        }

        private static byte[] Decompress(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip),
                CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }
        private async void DownloadTagList(List<Tags> tagList)
        {

        }

        /// <summary>
        /// appends a path to a given URL
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="pathFragment"></param>
        /// <returns></returns>
        private Uri buildURLPath(string URL, string pathFragment)
        {
            Uri Uri = new Uri(URL);
            UriBuilder baseUri = new UriBuilder(Uri);
            baseUri.Path = Uri.AbsolutePath + "/" + pathFragment;
            Uri finalUrl = baseUri.Uri;
            return finalUrl;
        }
    }
}

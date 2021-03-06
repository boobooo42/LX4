﻿using HtmlAgilityPack;
using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Text.RegularExpressions;

namespace LexicalAnalyzer.Scrapers
{
    /// <summary>
    /// Scraper for the Project Gutenberg website
    /// </summary>
    public class TextScraper : IScraper
    {
        private Guid m_guid;
        private string m_status;
        private float m_progress;
        private int m_priority;
        private ICorpusContext m_context;
        private int m_downloadCount;
        private int m_downloadLimit;
        private Stopwatch m_timer;
        private int m_timeLimit;
        private int m_corpusId;
        private int m_offset;
        private string m_userGivenName;
        private List<KeyValueProperty> m_properties;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <returns></returns>
        public TextScraper(ICorpusContext context)
        {
            m_guid = System.Guid.NewGuid();
            m_status = "init";
            m_progress = 0.0f;
            m_priority = 0;
            m_context = context;
            m_downloadCount = 0;
            m_downloadLimit = 0;
            m_timer = new Stopwatch();
            m_timeLimit = 0;
            Offset = 0;
        }

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
            get { return "Text Scraper"; }
        }
        public string TypeName { get { return "Text Scraper"; } }
        /// <summary>
        /// Gets description--is hardcoded
        /// </summary>
        /// <returns></returns>
        public static string Description
        {
            get
            {
                return
                    @"A scraper used for scraping the project
                    gutenberg website.";
            }
        }
        public string Desc
        {
            get { return @"A scraper used for scraping the project gutenberg website."; }
        }

        /// <summary>
        /// Gets content type --is hardcoded
        /// </summary>
        /// <returns></returns>
        public static string ContentType
        {
            get
            {
                return "text";
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

        public string UserGivenName
        {
            get
            {
                return m_userGivenName;
            }

            set
            {
                m_userGivenName = value;
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
                            "timelimit",  /* key */
                            "",  /* defaultValue */
                            "seconds"  /* type */
                            ));
                properties.Add(
                        new KeyValueProperty(
                            "downloadlimit",  /* key */
                            "",  /* defaultValue */
                            "items"  /* type */
                            ));
                properties.Add(
                        new KeyValueProperty(
                            "website",  /* key */
                            "http://www.gutenberg.org/robot/harvest",  /* defaultValue */
                            "url"  /* type */
                            ));
                properties.Add(
                        new KeyValueProperty(
                            "offset",  /* key */
                            "0",  /* defaultValue */
                            "> 40000"  /* type */
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
            get { return m_properties; }
            set
            {
                foreach (var property in value)
                {
                    if (property.Key == "timelimit")
                        TimeLimit = int.Parse(property.Value);
                    else if (property.Key == "downloadlimit")
                        DownloadLimit = int.Parse(property.Value);
                    else if (property.Key == "UserGivenName")
                        UserGivenName = property.Value;
                    else if (property.Key == "corpus")
                        CorpusId = int.Parse(property.Value);
                    else if (property.Key == "offset")
                        Offset = int.Parse(property.Value);


                }
                m_properties = new List<KeyValueProperty>(value);
            }
        }

        public int CorpusId
        {
            get
            {
                return m_corpusId;
            }

            set
            {
                m_corpusId = value;
            }
        }

        public int Offset
        {
            get
            {
                return m_offset;
            }

            set
            {
                m_offset = value;
            }
        }

        #region linkScraper

        /// <summary>
        /// Gets a task from a url, base method to create HTTP connection
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        static async Task<string> AsyncUrlToTask(string URL)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);

                //none of this filters it at all
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));

                // HTTP GET

                HttpResponseMessage response = await client.GetAsync(URL);
                HttpContent content = response.Content;

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("We did it, reddit!\n");
                    return await response.Content.ReadAsStringAsync();
                }
                return null;
            }
        }

        /// <summary>
        /// returns a list of files or links from a webpage based
        /// on the given xpath expression
        /// XPATH examples - "//a[contains(@href,'.zip')]" "//a[contains(@href,'.deb')]"
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="XPATHexpression"></param>
        /// <returns></returns>
        List<String> GetLinksFromPage(string URL, string XPATHexpression)
        {
            HtmlDocument testDoc = new HtmlDocument();
            List<string> docList = new List<string>();
            Task<string> task = AsyncUrlToTask(URL);

            try
            {
                task.Wait();
                testDoc.LoadHtml(task.Result);

                foreach (HtmlNode link in testDoc.DocumentNode.SelectNodes(XPATHexpression))
                {

                    docList.Add(link.GetAttributeValue("href", string.Empty));
                }
            }

            catch { }

            return docList;
        }
        #endregion

        #region download
        /// <summary>
        /// downloads the selected file to a memorystream, can work with any fileType
        /// </summary>
        /// <param name="downloadURL"></param>
        async Task<MemoryStream> loadFileToStream(string downloadURL)
        {

            HttpClient client = new HttpClient();

            var textArray = await client.GetByteArrayAsync(downloadURL);


            MemoryStream memoryStream = new MemoryStream(textArray);
            Random rand = new Random(DateTime.Now.Millisecond);
            await Task.Delay(rand.Next(250, 750));//waits between .25 and .75 seconds before trying next download

            return memoryStream;
        }

        /// <summary>
        /// Downloads zip file from url, extracts it, and loads its content into the database
        /// to byte arrays
        /// </summary>
        /// <param name="url"></param>
        void downloadZipFilesFromLinks(string url)
        {
            using (MemoryStream download = (loadFileToStream(url).Result))
            {
                extractAndLoadZipIntoDatabase(download, ".txt", url);
            }
        }

        #endregion

        #region extract and load

        /// <summary>
        /// extracts the given zip file and loads its contents into the database
        /// </summary>
        /// <param name="zipStream"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        void extractAndLoadZipIntoDatabase(MemoryStream zipStream, string fileType, string downloadURL)
        {
            Stream unzippedEntryStream;  //Unzipped data from a file in the archive
            ZipArchive archive = new ZipArchive(zipStream);
            bool lookingForFirstBook = true;
            //adds txt files to list of streams
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                try
                {
                    if (entry.FullName.EndsWith(fileType, StringComparison.OrdinalIgnoreCase) && lookingForFirstBook)
                    {

                        unzippedEntryStream = entry.Open(); // .Open will return a stream                                                      
                        byte[] byteArray = ReadFully(unzippedEntryStream); //converts stream to byte array

                        string bookName = getTitle(byteArray);

                        DateTime sqlDate = DateTime.Now;
                        ScraperUtilities.addCorpusContent(bookName, "text",
                            this.m_guid, this.GetType().FullName, sqlDate, downloadURL,
                            byteArray, m_context, m_corpusId);


                        unzippedEntryStream.Dispose();
                        lookingForFirstBook = false;
                    }

                }
                catch { } //ignore invalid files
            }
        }

        /// <summary>
        /// gets book title
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        string getTitle(byte[] byteArray)
        {
            string text = System.Text.Encoding.UTF8.GetString(byteArray);
            string header = text.Substring(0, 500);
            //here we are getting the link to the next page using  a regex split
            string eBookHeader = "The Project Gutenberg eBook,";
            string eTextHeader = "The Project Gutenberg";
            string title = "Project Gutenberg File";
            if (header.Contains(eBookHeader) || header.Contains(eTextHeader))
            {
                var lines = header.Split('\r');
                string firstLine = lines[0];
                int firstOf;
                int beginTitle;

                if (header.Contains(eBookHeader))
                {
                    string book = "eBook, ";
                    firstOf = firstLine.IndexOf(book);
                    beginTitle = firstOf + book.Length - 1;
                }
                else
                {
                    string start = "of ";
                    firstOf = firstLine.IndexOf(start);
                    beginTitle = firstOf + start.Length - 1;
                }

                title = firstLine.Substring(beginTitle);

            }

            return title;
        }

        /// <summary>
        /// converts a Stream to a byte array
        /// from: http://stackoverflow.com/a/221941
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        #endregion

        #region Project Gutenberg website mapping

        /// <summary>
        /// Gets the URL for the next page on gutenberg/robot/harvest
        /// </summary>
        /// <param name="linksFromPage"></param>
        /// <returns></returns>
        string getNextPage(List<string> linksFromPage)
        {
            //link to next page should be found at the bottom of list
            //changethis to a foreach if PG layout changes
            if (linksFromPage[linksFromPage.Count - 1].Contains("offset=") && linksFromPage.Count > 0)
                return "http://www.gutenberg.org/robot/" + linksFromPage[linksFromPage.Count - 1];
            else return null;
        }

        /// <summary>
        /// filters a list of links by the given filetype that the link ends with
        /// </summary>
        /// <param name="pageLinks"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        List<string> getListOfDownloadsForPage(List<string> pageLinks, string fileType)
        {
            List<string> listOfDownloads = new List<string>();
            foreach (string link in pageLinks)
            {
                if (link.EndsWith(fileType))
                    listOfDownloads.Add(link);
            }
            return listOfDownloads;
        }

        #endregion

        #region run
        /// <summary>
        /// Runs the text scraper
        /// </summary>
        /// <returns></returns>
        public void Run()
        {
            string rootURL = "";
            foreach (KeyValueProperty i in DefaultProperties)
                if (i.Key.Equals("website"))
                    rootURL = i.Value;
            if(m_offset != 0)
            {

                rootURL += "?offset=" + m_offset;
            }
            scrapePG(rootURL);
        }

        /// <summary>
        /// Scrapes the Project Gutenberg website
        /// </summary>
        /// <param name="currentURL"></param>
        /// <param name="pagesToGet"></param>
        public void scrapePG(string currentURL)
        {
            m_downloadCount = 0;
            m_timer.Reset();
            bool downloadLimitReached = downloadStop();
            bool timeLimitReached = timeStop();
            m_timer.Start();
            while (!downloadLimitReached && !timeLimitReached)
            {
                List<string> tempLinkList = GetLinksFromPage(currentURL, "//a[@href]");
                var dlList = getListOfDownloadsForPage(tempLinkList, ".zip");
                foreach (string url in dlList)
                {
                    downloadZipFilesFromLinks(url);
                    m_downloadCount++;
                    m_progress = (float)m_downloadCount / m_downloadLimit;
                    downloadLimitReached = downloadStop();
                    timeLimitReached = timeStop();
                    if (downloadLimitReached || timeLimitReached)
                        break;
                }
                currentURL = getNextPage(tempLinkList);
            }
            if (downloadLimitReached && timeLimitReached)
                m_status = ScraperUtilities.SCRAPER_STATUS_TIME_AND_DOWNLOAD_LIMIT_REACHED;
            else if (downloadLimitReached)
                m_status = ScraperUtilities.SCRAPER_STATUS_DOWNLOAD_LIMIT_REACHED;
            else if (timeLimitReached)
                m_status = ScraperUtilities.SCRAPER_STATUS_TIME_LIMIT_REACHED;
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

        #endregion
    }
}

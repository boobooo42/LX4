using HtmlAgilityPack;
using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using LexicalAnalyzer.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Data.SqlTypes;
using System.Security.Cryptography;
using System.Text;

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
                            "http://www.gutenberg.org/robot/harvest",  /* defaultValue */
                            "url"  /* type */
                            ));
                properties.Add(
                        new KeyValueProperty(
                            "filesToDownload", /* key */
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
            get ; set;
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
            await Task.Delay(rand.Next(5000, 7000));//wait 5 seconds before trying next download

            return memoryStream;
        }

        /// <summary>
        /// Downloads zip files from urls, extracts them, and loads their contents into the database
        /// to byte arrays
        /// </summary>
        /// <param name="urls"></param>
        void downloadZipFilesFromLinks(List<string> urls)
        {
            foreach (string downloadURL in urls)
            {
                using (MemoryStream download = (loadFileToStream(downloadURL).Result))
                {
                    extractAndLoadZipIntoDatabase(download, ".txt",downloadURL);
                }
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

            //adds txt files to list of streams
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                try
                {
                    if (entry.FullName.EndsWith(fileType, StringComparison.OrdinalIgnoreCase))
                    {

                        unzippedEntryStream = entry.Open(); // .Open will return a stream                                                        //Process entry data here
                        byte[] byteArray = ReadFully(unzippedEntryStream); //converts stream to byte array
                     //   ScraperUtilities.loadByteArrayIntoDatabase(byteArray);



                        SqlDateTime sqlDate = new SqlDateTime(DateTime.Now);
                        ScraperUtilities.addCorpusContent(-1, "", ".txt", "Project Gutenberg File"
                            , this.m_guid, this.GetType().FullName, sqlDate, downloadURL,
                            byteArray,m_context);


                        unzippedEntryStream.Dispose();
                    }
                   
                }
                catch {} //ignore invalid files
            }
        }

    //    /// <summary>
    //    /// creates a corpus content and adds it to the corpur content repository
    //    /// </summary>
    //    /// <param name="Id"></param>
    //    /// <param name="Hash"></param>
    //    /// <param name="Name"></param>
    //    /// <param name="Type"></param>
    //    /// <param name="ScraperGuid"></param>
    //    /// <param name="ScraperType"></param>
    //    /// <param name="DownloadDate"></param>
    //    /// <param name="DownloadURL"></param>
    //    /// <param name="Content"></param>
    //    /// <param name="corpContent"></param>
    //    void addCorpusContent(long Id, string Hash, string Name, string Type,
    //SqlGuid ScraperGuid, string ScraperType, SqlDateTime DownloadDate, string DownloadURL,
    //byte[] Content)
    //    {
    //        CorpusContent corpContent = new CorpusContent();

    //        /* creates hash of byte array*/
    //        using (MD5 md5Hash = MD5.Create())
    //        {
    //            // Convert the input string to a byte array and compute the hash.
    //            byte[] data = md5Hash.ComputeHash(Content);
    //            StringBuilder sBuilder = new StringBuilder();

    //            // Loop through each byte of the hashed data 
    //            // and format each one as a hexadecimal string.
    //            for (int i = 0; i < data.Length; i++)
    //            {
    //                sBuilder.Append(data[i].ToString("x2"));
    //            }
    //            Hash = sBuilder.ToString(); //change hash to real hash
    //        }

    //        corpContent.Id = Id;
    //        corpContent.Hash = Hash;
    //        corpContent.Name = Name;
    //        corpContent.Type = Type;
    //        corpContent.ScraperGuid = ScraperGuid;
    //        corpContent.ScraperType = ScraperType;
    //        corpContent.DownloadDate = DownloadDate;
    //        corpContent.DownloadURL = DownloadURL;
    //        corpContent.Content = Content;
    //        m_context.CorpusContentRepository.Add(corpContent);
    //    }

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
            if (linksFromPage[linksFromPage.Count - 1].Contains("offset="))
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
            Debug.Assert(false);
            string rootURL = "";
            foreach (KeyValueProperty i in DefaultProperties)
                if (i.Key.Equals("website"))
                    rootURL = i.Value;
            scrapePG(rootURL, 10);
        }

        /// <summary>
        /// Scrapes the Project Gutenberg website
        /// </summary>
        /// <param name="currentURL"></param>
        /// <param name="pagesToGet"></param>
        public void scrapePG(string currentURL, int pagesToGet)
        {
            for (int i = 0; i < pagesToGet; i++)
            {
                List<string> tempLinkList = GetLinksFromPage(currentURL, "//a[@href]");
                var dlList = getListOfDownloadsForPage(tempLinkList, ".zip");
                downloadZipFilesFromLinks(dlList);
                currentURL = getNextPage(tempLinkList);
            }

        }

        #endregion
    }
}

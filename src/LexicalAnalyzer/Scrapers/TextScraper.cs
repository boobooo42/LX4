using HtmlAgilityPack;
using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace LexicalAnalyzer.Scrapers
{
    /// <summary>
    /// Scraper for the Project Gutenberg website
    /// </summary>
    public class TextScraper : IScraper
    {

        /* Private members */
        private Guid m_guid;
        private string m_status;
        private float m_progress;
        private int m_priority;
        private List<KeyValueProperty> m_properties;

        public TextScraper()
        {
            m_guid = System.Guid.NewGuid();
            m_status = "init";
            m_progress = 0.0f;
            m_priority = 0;
            m_properties = new List<KeyValueProperty>();
            m_properties.Add(
                    new KeyValueProperty(
                        "timeout",  /* key */
                        "30",  /* defaultValue */
                        "seconds"  /* type */
                        ));
            m_properties.Add(
                    new KeyValueProperty(
                        "website",  /* key */
                        "http://www.gutenberg.org/robot/harvest",  /* defaultValue */
                        "url"  /* type */
                        ));
            m_properties.Add(
                new KeyValueProperty(
                    "filesToDownload", /* key */
                    "", /* defaultValue */
                    "urls" /* type */
                    ));
        }

        /* Public Interface */
        public Guid Guid
        {
            get
            {
                return m_guid;
            }
        }

        public string DisplayName
        {
            get { return "Text Scraper"; }
        }

        public string Description
        {
            get
            {
                return
                    @"A scraper used for scraping the project
                    gutenberg website.";
            }
        }

        public string ContentType
        {
            get
            {
                return "text";
            }
        }

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

        public float Progress
        {
            get
            {
                return m_progress;
            }
        }

        public int Priority
        {
            get
            {
                return m_priority;
            }
        }

        public IEnumerable<KeyValueProperty> Properties
        {
            get
            {
                return m_properties;
            }
        }
        public void Run()
        {
            Debug.Assert(false);
            List<string> links = scrapePG("http://www.gutenberg.org/robot/harvest", 10);
            
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
        /// Main method to download zip files from a page and extract the text files
        /// to byte arrays
        /// </summary>
        /// <param name="filesFromPage"></param>
        List<byte[]> downloadFilesFromPage(List<string> filesFromPage)
        {
            var fileList = new List<byte[]>();
            foreach (string downloadURL in filesFromPage)
            {
                using (MemoryStream download = (loadFileToStream(downloadURL).Result))
                {
                    fileList.AddRange(getFilesFromZipStream(download, ".txt"));
                }
            }
            return fileList;
        }

        #endregion

        #region extract

        /// <summary>
        /// gets files of the given type from a .zip 
        /// returns a list of byte arrays
        /// </summary>
        /// <param name="zipStream"></param>
        /// <returns></returns>
        List<byte[]> getFilesFromZipStream(MemoryStream zipStream, string fileType)
        {
            Stream unzippedEntryStream;  //Unzipped data from a file in the archive

            List<byte[]> listOfTextStreams = new List<byte[]>();
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
                        ScraperUtilities.loadByteArrayIntoDatabase(byteArray);
                        listOfTextStreams.Add(byteArray);
                        unzippedEntryStream.Dispose();
                    }
                }
                catch { }
            }


            return listOfTextStreams;
        }

        /// <summary>
        /// stores a text file in the database
        /// </summary>
        /// <param name="textFile"></param>
        void addTextFileToDatabase(byte[] textFile)
        {

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

        #region website mapping

        /// <summary>
        /// Gets the URL for the next page on gutenberg/robot/harvest
        /// </summary>
        /// <param name="URL"></param>
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

        /// <summary>
        /// returns a list of urls for .txt files from the ProjectGutenberg website
        /// the limits set the range of txt files to return
        /// </summary>
        /// <param name="lowerLimit"></param>
        /// <param name="upperLimit"></param>
        public List<string> scrapePG(string currentURL, int pagesToGet)
        {
            //the main URL for the project gutenberg depository, hope this doens't change
            //"http://www.gutenberg.org/robot/harvest";

            List<string> finalLinkList = new List<string>();


            for (int i = 0; i < pagesToGet; i++)
            {
                List<string> tempLinkList = GetLinksFromPage(currentURL, "//a[@href]");
                var dlList = getListOfDownloadsForPage(tempLinkList, ".zip");
                finalLinkList.AddRange(dlList);

                //This is where the downloading happens, uncomment if you want to download zip files
                List<byte[]> downloadedFiles = downloadFilesFromPage(dlList);
                currentURL = getNextPage(tempLinkList);
                finalLinkList.Add(currentURL);
            }

            return finalLinkList;

        }


    }
}

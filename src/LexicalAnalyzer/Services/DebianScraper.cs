using HtmlAgilityPack;
using LexicalAnalyzer.Resources;
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

namespace LexicalAnalyzer.Services
{
    public class DebianScraper : IScraper
    {
        /* Structs */
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

        /* Public Interface */
        public Guid Guid {
            get {
                return System.Guid.NewGuid();
            }
        }

        public string DisplayName {
            get { return "Debian Scraper"; }
        }
        public string Description {
            get {
                return
                    @"Useful for scraping .deb files from the Debian archive
                    mirrors. Files can be scraped for a variety of different
                    architectures, including x86 and x86_64.";
            }
        }
        public string ContentType {
            get {
                return "executables";
            }
        }

        public void Run() {
            /* TODO: Perform scraping here */
            /* TODO: We do not have a model for corpus content yet, so there is
             * nowhere to put anything we download yet */

            //  string debs = GetBinaries(myURL);

            List<string> downLoadTypes = new List<string>();
            downLoadTypes.Add(".deb");
            downLoadTypes.Add(".tar.gz");

            string debs = "";
            
            //work being done here
            downLoadTypes = GetDownloads(downLoadTypes,urlList);

            foreach (string s in downLoadTypes)
            {
                debs += s + "\n";
            }
        }

        public float Progress {
            get {
                return 0.0f;
            }
        }

        /* Internal Methods */
        /* DocTree */
        /// <summary>
        /// Gets a MD5 hash of a HtmlDocument
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        string getHash(HtmlDocument doc)
        {
            string docAsString = doc.DocumentNode.OuterHtml;
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(docAsString);
            MD5 docHash = MD5.Create();
            return Convert.ToBase64String(docHash.ComputeHash(inputBytes));
        }

        /// <summary>
        /// Returns all the urls in the HtmlDocument
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
         List<string> getLinks(HtmlDocument doc)
        {
            List<string> urls = new List<string>();
            if (doc.DocumentNode.SelectNodes(LinkPath) != null)
            {
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes(LinkPath))
                {
                    urls.Add(link.GetAttributeValue("href", string.Empty));
                }
            }

            return urls;
        }

        /// <summary>
        /// Returns a restricted list of the urls in the HtmlDocument that are
        /// within the given domain
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        List<string> getInnerLinks(HtmlDocument doc, string domain)
        {
            List<string> links = getLinks(doc);
            List<string> innerLinks = new List<string>();
            foreach (string link in links)
            {
                if (link.Contains("http://") || link.Contains("https://"))
                {
                    if (link.Contains(rootURL))
                        innerLinks.Add(link);
                }
                else
                    innerLinks.Add(rootURL + link);
            }
            return innerLinks;
        }

        /// <summary>
        /// Wrapper function for creating the HtmlDocument tree
        /// </summary>
        /// <param name="root"></param>
        /// <param name="url"></param>
        void createHtmlDocTree(HtmlDocument root, string url)
        {
            htmlDocumentTree = new HtmlDocumentTree(root, url);
            List<string> htmlDocumentHashes = new List<string>();
            htmlDocumentHashes.Add(url);
            createHtmlDocTreeSubroutine(htmlDocumentTree, htmlDocumentHashes);
        }

        /// <summary>
        /// Recursively builds the HtmlDocument tree
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="hashedDocs"></param>
        private void createHtmlDocTreeSubroutine(HtmlDocumentTree tree, List<string> hashedDocs)
        {
            List<string> innerLink = getInnerLinks(tree.Node, tree.Url);
            List<HtmlDocumentTree> childrenToAdd = new List<HtmlDocumentTree>();
            foreach (string link in innerLink)
            {
                //string docHash = getHash(doc);
                if (!hashedDocs.Contains(link))
                {
                    hashedDocs.Add(link);
                    Task<string> task = AsyncUrlToTask(link);
                    task.Wait();
                    var doc = new HtmlDocument();
                    doc.LoadHtml(task.Result);
                    childrenToAdd.Add(new HtmlDocumentTree(doc, link));
                }
            }
            foreach (HtmlDocumentTree child in childrenToAdd)
            {
                tree.ChildDocuments.Add(child);
            }
            foreach (HtmlDocumentTree child in childrenToAdd)
            {
                createHtmlDocTreeSubroutine(child, hashedDocs);
            }
        }

        /// <summary>
        /// Gets a task from a url async
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        private static async Task<string> AsyncUrlToTask(string URL)
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
                return "it failed";
            }
        }

        /* Display */
        private string displayAllUrls(HtmlDocument doc)
        {
            string result = "";
            List<string> allUrls = getLinks(doc);
            foreach (string url in allUrls)
                result += url + "\n";
            return result;
        }

        private string displayInnerUrls(HtmlDocument doc)
        {
            string result = "";
            List<string> innerUrls = getInnerLinks(doc, "osuosl.org");
            foreach (string url in innerUrls)
                result += url + "\n";
            return result;
        }

        private string displayHtmlDocumentTree(HtmlDocumentTree tree)
        {
            string result = "";
            foreach (HtmlDocumentTree child in tree.ChildDocuments)
            {
                result += child.Url + "\n";
            }
            urlList = new List<string>();
            exeLinks = "";

            displayHtmlDocumentTreeSubroutine(tree, "", 0);
            return exeLinks;
        }

        void displayHtmlDocumentTreeSubroutine(
                HtmlDocumentTree tree, string result, int level)
        {
            urlList.Add(tree.Url);

            if (tree.ChildDocuments == null || tree.ChildDocuments.Count == 0)
            {

                exeLinks += result + "\n";
                urlList.Add(tree.Url);

                return;
            }

            foreach (var child in tree.ChildDocuments)
            {
                displayHtmlDocumentTreeSubroutine(child, child.Url, level + 1);
            }
        }

        /// <summary>
        /// new method to clean up get method in controller
        /// </summary>
        /// <returns></returns>
        public string RunDisplay()
        {
            //   string baseUrl = "http://debian.osuosl.org/debian/pool/main/c/";
            Task<string> task = AsyncUrlToTask(rootURL);
            task.Wait();

            var testDoc = new HtmlDocument();
            testDoc.LoadHtml(task.Result);
            createHtmlDocTree(testDoc, rootURL);
            //List<string> htmlDocumentHashes = new List<string>();
            //htmlDocumentHashes.Add(getHash(testDoc));
            //HtmlDocumentTree tree = createHtmlDocTree(new HtmlDocumentTree(testDoc, rootURL), htmlDocumentHashes);
            string result = "";
            //result += tree.ChildDocuments.Count.ToString() + "\n";
            result += displayHtmlDocumentTree(htmlDocumentTree);
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

        /* Binary Code */
        /// <summary>
        /// method to retrive binaries from base webpage
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public string GetBinaries(string URL)
        {
            var testDoc = new HtmlDocument();


            Task<string> task = AsyncUrlToTask(URL);

            task.Wait();

            testDoc.LoadHtml(task.Result);
            string binaries = "";


            foreach (HtmlNode link in testDoc.DocumentNode.SelectNodes(DownloadPath))
            {
                binaries += link.GetAttributeValue("href", string.Empty) + "\n";
            }

            string urlCount = DownCollection.Count().ToString();

            return binaries;

        }

        /// <summary>
        /// Method for retrieving binary files, overloaded
        /// </summary>
        /// <returns></returns>
        public List<string> GetDownloads(List<string> docTypes, List<string> linkList)
        {
            List<string> downloadList = new List<string>();
            //foreach (string link in urlList)
            //{
            //    if (link.EndsWith(".deb") || link.EndsWith(".gz")
            //        || link.EndsWith(".xz"))
            //    {
            //        result += link + "\n";
            //    }
            //}

            foreach (string link in linkList)
            {
                foreach (string docType in docTypes)
                {
                    if (link.EndsWith(docType)) downloadList.Add(link);
                }
            }
            return downloadList;
        }

        /* Download */
        void downloadFiles(List<string> urlsToDownload)
        {
            List<splitUrl> splitUrls = new List<splitUrl>();
            foreach (string url in urlsToDownload)
            {
                splitUrls.Add(new splitUrl(
                            url.Substring(0, url.LastIndexOf('/') + 1),
                            url.Substring(url.LastIndexOf('/') + 1)));
            }

            HttpClient client = new HttpClient();

            foreach (splitUrl su in splitUrls)
            {
                client.GetAsync(su.baseUrl).ContinueWith(
                   (requestTask) =>
                   {
                   // Get HTTP response from completed task.
                   HttpResponseMessage response = requestTask.Result;
                       try
                       {
                           // Check that response was successful or throw exception
                           response.EnsureSuccessStatusCode();

                           // Read response asynchronously and save to file
                           ReadAsFileAsync(response.Content, su.file, true);
                       }
                       catch { }
                   });
            }
        }

        // code from:
        // <https://blogs.msdn.microsoft.com/henrikn/2012/02/17/httpclient-downloading-to-a-local-file/>
        static Task ReadAsFileAsync(
                HttpContent content, string filename, bool overwrite)
        {
            string directory = "J:\\Desktop\\Output\\"; // you can change this
            string pathname = directory + filename; 
            //string pathname = Path.GetFullPath(filename); //will put in default directory
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(
                        pathname,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None);
                return content.CopyToAsync(fileStream).ContinueWith(
                    (copyTask) =>
                    {
                        fileStream.Dispose();
                    });
            }
            catch
            {
                if (fileStream != null)
                {
                    fileStream.Dispose();
                }

                throw;
            }
        }
    }
}

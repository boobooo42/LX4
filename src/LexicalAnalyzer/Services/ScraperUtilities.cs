using HtmlAgilityPack;
using LexicalAnalyzer.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace LexicalAnalyzer.Services
{
    public static class ScraperUtilities
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
        /* Download */
        static void downloadFiles(List<string> urlsToDownload)
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
        public static Task ReadAsFileAsync(
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

        /* Display */
        private static  string displayAllUrls(HtmlDocument doc, string linkPath)
        {
            string result = "";
            List<string> allUrls = getLinks(doc, linkPath);
            foreach (string url in allUrls)
                result += url + "\n";
            return result;
        }

        private static string displayInnerUrls(HtmlDocument doc, string linkPath, string rootURL)
        {
            string result = "";
            List<string> innerUrls = getInnerLinks(doc, linkPath, rootURL);
            foreach (string url in innerUrls)
                result += url + "\n";
            return result;
        }

        public static string displayHtmlDocumentTree(HtmlDocumentTree tree)
        {
            string result = "";
            foreach (HtmlDocumentTree child in tree.ChildDocuments)
            {
                result += child.Url + "\n";
            }
            List<string> urlList = new List<string>();
            string exeLinks = "";

            return displayHtmlDocumentTreeSubroutine(tree, "", 0, urlList, exeLinks);
        }

        static string displayHtmlDocumentTreeSubroutine(
                HtmlDocumentTree tree, string result, int level, List<string> urlList, string exeLinks)
        {
            urlList.Add(tree.Url);

            if (tree.ChildDocuments == null || tree.ChildDocuments.Count == 0)
            {

                exeLinks += result + "\n";
                urlList.Add(tree.Url);
            }

            foreach (var child in tree.ChildDocuments)
            {
                displayHtmlDocumentTreeSubroutine(child, child.Url, level + 1, urlList, exeLinks);
            }
            return exeLinks;
        }

        /// <summary>
        /// Returns all the urls in the HtmlDocument
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        static List<string> getLinks(HtmlDocument doc, string LinkPath)
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
        public static List<string> getInnerLinks(HtmlDocument doc, string linkPath, string rootURL)
        {
            List<string> links = getLinks(doc, linkPath);
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

        /* Internal Methods */
        /* DocTree */
        /// <summary>
        /// Gets a MD5 hash of a HtmlDocument
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static string getHash(HtmlDocument doc)
        {
            string docAsString = doc.DocumentNode.OuterHtml;
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(docAsString);
            MD5 docHash = MD5.Create();
            return Convert.ToBase64String(docHash.ComputeHash(inputBytes));
        }

        /// <summary>
        /// Gets a task from a url async
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public static async Task<string> AsyncUrlToTask(string URL)
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

        /// <summary>
        /// Wrapper function for creating the HtmlDocument tree
        /// </summary>
        /// <param name="root"></param>
        /// <param name="url"></param>
        public static HtmlDocumentTree createHtmlDocTree(HtmlDocument root, string url, string LinkPath, string rootURL)
        {
            HtmlDocumentTree htmlDocumentTree = new HtmlDocumentTree(root, url);
            List<string> htmlDocumentHashes = new List<string>();
            htmlDocumentHashes.Add(url);
            return createHtmlDocTreeSubroutine(htmlDocumentTree, htmlDocumentHashes, LinkPath, rootURL);
        }

        /// <summary>
        /// Recursively builds the HtmlDocument tree
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="hashedDocs"></param>
        private static HtmlDocumentTree createHtmlDocTreeSubroutine(HtmlDocumentTree tree, List<string> hashedDocs, string LinkPath, string rootURL)
        {
            List<string> innerLink = ScraperUtilities.getInnerLinks(tree.Node, LinkPath, rootURL);
            List<HtmlDocumentTree> childrenToAdd = new List<HtmlDocumentTree>();
            foreach (string link in innerLink)
            {
                //string docHash = getHash(doc);
                if (!hashedDocs.Contains(link))
                {
                    hashedDocs.Add(link);
                    Task<string> task = ScraperUtilities.AsyncUrlToTask(link);
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
                return createHtmlDocTreeSubroutine(child, hashedDocs, LinkPath, rootURL);
            }
            return tree;
        }

        /* Binary Code */
        /// <summary>
        /// method to retrive binaries from base webpage
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public static string GetBinaries(string URL, string DownloadPath, HtmlNodeCollection DownCollection)
        {
            var testDoc = new HtmlDocument();


            Task<string> task = ScraperUtilities.AsyncUrlToTask(URL);

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
        public static List<string> GetDownloads(List<string> docTypes, List<string> linkList)
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
    }
}

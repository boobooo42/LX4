using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.IO;
using LexicalAnalyzer.Resources;
using System.Text.RegularExpressions;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LexicalAnalyzer.Controllers
{
    [Route("api/[controller]")]
    public class ScraperController : Controller
    {

        #region Global Variables
        static string myURL = "http://debian.osuosl.org/debian/pool/main/liby/";//"http://debian.osuosl.org/debian/pool/main/liby/";
        string DownloadPath = "//a[contains(@href,'.deb')]";
        static string LinkPath = "//a[@href]";
        static string exeLinks;
        static List<string> urlList;
        static HtmlDocumentTree htmlDocumentTree;
        List<HtmlNode> DownLinkList;
        HtmlNodeCollection DownCollection;
        #endregion

        #region DocTree
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
        /// Returns a restricted list of the urls in the HtmlDocument that are within the given domain
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
                    if (link.Contains(myURL))
                        innerLinks.Add(link);
                }
                else
                    innerLinks.Add(myURL + link);
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
        void createHtmlDocTreeSubroutine(HtmlDocumentTree tree, List<string> hashedDocs)
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
            foreach(HtmlDocumentTree child in childrenToAdd)
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
        static async Task<string> AsyncUrlToTask(string URL)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
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
        #endregion

        #region Display
        string displayAllUrls(HtmlDocument doc)
        {
            string result = "";
            List<string> allUrls = getLinks(doc);
            foreach (string url in allUrls)
                result += url + "\n";
            return result;
        }
        string displayInnerUrls(HtmlDocument doc)
        {
            string result = "";
            List<string> innerUrls = getInnerLinks(doc, "osuosl.org");
            foreach (string url in innerUrls)
                result += url + "\n";
            return result;
        }
         string displayHtmlDocumentTree(HtmlDocumentTree tree)
        {
            string result = "";
            foreach(HtmlDocumentTree child in tree.ChildDocuments)
            {
                result += child.Url + "\n";
            }
            urlList = new List<string>();
            exeLinks = "";

            displayHtmlDocumentTreeSubroutine(tree, "", 0);
            return exeLinks;
        }


         void displayHtmlDocumentTreeSubroutine(HtmlDocumentTree tree, string result, int level)
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
            Task<string> task = AsyncUrlToTask(myURL);
            task.Wait();

            var testDoc = new HtmlDocument();
            testDoc.LoadHtml(task.Result);
            createHtmlDocTree(testDoc, myURL);
            //List<string> htmlDocumentHashes = new List<string>();
            //htmlDocumentHashes.Add(getHash(testDoc));
            //HtmlDocumentTree tree = createHtmlDocTree(new HtmlDocumentTree(testDoc, myURL), htmlDocumentHashes);
            string result = "";
            //result += tree.ChildDocuments.Count.ToString() + "\n";
            result += displayHtmlDocumentTree(htmlDocumentTree);
            // result = displayAllUrls(testDoc);
            //result += displayInnerUrls(testDoc);
            //result += getFiles(testDoc);

            result = "";

            //this just lists all the parent node urls and the end of the
            foreach (var x in urlList)
            {
                result += x + "\n";
            }

            return result;
        }

        #endregion

        #region Binary Code

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
        public string GetBinaries()
        {
            string result = "";
            foreach (string link in urlList)
            {
                if (link.EndsWith(".deb") || link.EndsWith(".gz")
                    || link.EndsWith(".xz")){
                    result += link + "\n";
                }
            }
            return result;
        }

        #endregion

        #region Controllers
        // GET: api/scraper
        [HttpGet]
        public string Get()
        {
            string result = RunDisplay();

            return result;
        }



        // GET api/scraper/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            //  string debs = GetBinaries(myURL);
            string debs = GetBinaries();

            return debs;
        }

        // POST api/scraper
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        #endregion

    }
}

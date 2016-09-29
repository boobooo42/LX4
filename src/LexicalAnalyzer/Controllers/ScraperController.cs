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
        static string myURL = "http://debian.osuosl.org/debian/pool/main/liby/";
        string DownloadPath = "//a[contains(@href,'.deb')]";
       static string LinkPath = "//a[@href]";
       static string exeLinks;
        List<HtmlNode> DownLinkList;
        HtmlNodeCollection DownCollection;
        #endregion

        static string getHash(HtmlDocument doc)
        {
            string docAsString = doc.DocumentNode.OuterHtml;
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(docAsString);
            MD5 docHash = MD5.Create();
            return Convert.ToBase64String(docHash.ComputeHash(inputBytes));
        }

        static List<string> getLinks(HtmlDocument doc)
        {
            List<string> urls = new List<string>();
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes(LinkPath))
            {
                urls.Add(link.GetAttributeValue("href", string.Empty));
            }

            return urls;
        }
        static List<string> getInnerLinks(HtmlDocument doc, string domain)
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
        static HtmlDocumentTree createHtmlDocTree(HtmlDocumentTree tree, List<string> hashedDocs)
        {
            List<string> innerLink = getInnerLinks(tree.Node, myURL);
            foreach (string link in innerLink)
            {
                Task<string> task = AsyncUrlToTask(link);
                task.Wait();
                var doc = new HtmlDocument();
                doc.LoadHtml(task.Result);
                string docHash = getHash(doc);
                if (!hashedDocs.Contains(docHash))
                {
                    hashedDocs.Add(docHash);
                    tree.ChildDocuments.Add(new HtmlDocumentTree(doc, link));
                }
            }
            return tree;
        }

        static async Task<string> AsyncUrlToTask(string URL)
        {


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                //  client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


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
  
        #region Display
        static string displayAllUrls(HtmlDocument doc)
        {
            string result = "";
            List<string> allUrls = getLinks(doc);
            foreach (string url in allUrls)
                result += url + "\n";
            return result;
        }
        static string displayInnerUrls(HtmlDocument doc)
        {
            string result = "";
            List<string> innerUrls = getInnerLinks(doc, "osuosl.org");
            foreach (string url in innerUrls)
                result += url + "\n";
            return result;
        }
        static string displayHtmlDocumentTree(HtmlDocumentTree tree)
        {
            string result = "";
            foreach(HtmlDocumentTree child in tree.ChildDocuments)
            {
                result += child.Url + "\n";
            }
            exeLinks = "";

            displayHtmlDocumentTreeSubroutine(tree, "", 0);
            return exeLinks;
        }


        static void displayHtmlDocumentTreeSubroutine(HtmlDocumentTree tree, string result, int level)
        {
            
            if(tree.ChildDocuments == null || tree.ChildDocuments.Count == 0)
            {
                exeLinks += result + "\n";
                return;
            }

            foreach (var child in tree.ChildDocuments)
            {
                displayHtmlDocumentTreeSubroutine(child, result + child.Url, level + 1);
            }

            //for (int i = 0; i < tree.ChildDocuments.Count; i++)
            //{
            //    result += displayHtmlDocumentTreeSubroutine(tree.ChildDocuments[i], result, level + 1) + "\n";
            //}

        }



        #endregion

        #region Binary Code
        public string GetBinaries(string URL)
        {
            var testDoc = new HtmlDocument();


            Task<string> task = AsyncUrlToTask(URL);

            task.Wait();

            testDoc.LoadHtml(task.Result);
            string binaries = "";

       //     DownCollection = new HtmlNodeCollection(testDoc.DocumentNode);

            foreach (HtmlNode link in testDoc.DocumentNode.SelectNodes(DownloadPath))
            {
                binaries += link.GetAttributeValue("href", string.Empty) + "\n";

            }

            //foreach (char s in task.Result)
            //{
            //    urls += s;
            //}


            string urlCount = DownCollection.Count().ToString();

            return binaries;

        }

        #endregion

        #region Controllers
        // GET: api/scraper
        [HttpGet]
        public string Get()
        {
         //   string baseUrl = "http://debian.osuosl.org/debian/pool/main/c/";
            Task<string> task = AsyncUrlToTask(myURL);
            task.Wait();

            var testDoc = new HtmlDocument();
            testDoc.LoadHtml(task.Result);
            List<string> htmlDocumentHashes = new List<string>();
            htmlDocumentHashes.Add(getHash(testDoc));
            HtmlDocumentTree tree = createHtmlDocTree(new HtmlDocumentTree(testDoc, myURL), htmlDocumentHashes);
            string result = "";
            result += tree.ChildDocuments.Count.ToString() + "\n";
            result += displayHtmlDocumentTree(tree);
           // result = displayAllUrls(testDoc);
            //result += displayInnerUrls(testDoc);
            //result += getFiles(testDoc);
            return result;
        }



        // GET api/scraper/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            string debs = GetBinaries(myURL);

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

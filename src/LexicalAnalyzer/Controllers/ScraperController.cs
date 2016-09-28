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

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LexicalAnalyzer.Controllers
{
    [Route("api/[controller]")]
    public class ScraperController : Controller
    {

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
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                urls.Add(link.GetAttributeValue("href", string.Empty));
            }
            return urls;
        }
        static List<string> getInnerLinks(HtmlDocument doc, string domain)
        {
            List<string> urls = getLinks(doc);
            List<string> innerLinks = new List<string>();
            foreach (string url in urls)
                if (url.Contains(domain))
                    innerLinks.Add(url);
            return innerLinks;
        }
        static HtmlDocumentTree createHtmlDocTree(HtmlDocumentTree tree, List<string> hashedDocs)
        {
            List<string> innerUrls = getInnerLinks(tree.Node, "osuosl.org");
            foreach (string url in innerUrls)
            {
                if (url != tree.Url)
                {
                    Task<string> task = urlToTaskAsync(url);
                    task.Wait();
                    var doc = new HtmlDocument();
                    doc.LoadHtml(task.Result);
                    string docHash = getHash(doc);
                    if (!hashedDocs.Contains(docHash))
                    {
                        hashedDocs.Add(docHash);
                        tree.ChildDocuments.Add(new HtmlDocumentTree(doc, url));
                    }
                }
            }
            return tree;
        }
        #region old
        static string getFiles(HtmlDocument doc)
        {
            string urls = "";
            string fileType = "img";
            HtmlNodeCollection files = new HtmlNodeCollection(doc.DocumentNode.ParentNode);
            files = doc.DocumentNode.SelectNodes("//" + fileType);

            if (files != null)
            {
                foreach (HtmlNode file in files)
                {
                    HtmlAttribute src = file.Attributes[@"src"];
                    urls += src.Value + "\n";
                }
                return urls;
            }
            else
                return "No " + fileType + " files found";
        }
        #endregion

        static async Task<string> urlToTaskAsync(string url)
        {                     
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(new Uri(url).AbsoluteUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
                
                // HTTP GET
                HttpResponseMessage response = await client.GetAsync("/");
                if (response.IsSuccessStatusCode)
                {
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
                result += url;
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
            return displayHtmlDocumentTreeSubroutine(tree, "", 0);
        }
        static string displayHtmlDocumentTreeSubroutine(HtmlDocumentTree tree, string result, int level)
        {
            result += "Level " + level.ToString() + ": " + tree.Url + "\n";
            for(int i = 0; i < tree.ChildDocuments.Count; i++)
            {
                result += displayHtmlDocumentTreeSubroutine(tree.ChildDocuments[i], result, level + 1) + "\n";
            }
            return result;
        }
        #endregion

        // GET: api/scraper
        [HttpGet]
        public string Get()
        {
            string baseUrl = "http://debian.osuosl.org/debian/pool/main/c/";
            Task<string> task = urlToTaskAsync(baseUrl);
            task.Wait();

            var testDoc = new HtmlDocument();
            testDoc.LoadHtml(task.Result);
            List<string> htmlDocumentHashes = new List<string>();
            htmlDocumentHashes.Add(getHash(testDoc));
            HtmlDocumentTree tree = createHtmlDocTree(new HtmlDocumentTree(testDoc, baseUrl), htmlDocumentHashes);
            string result = "";
            result += tree.ChildDocuments.Count.ToString() + "\n";
            result += displayHtmlDocumentTree(tree);
            //result = displayAllUrls(testDoc);
            //result += displayInnerUrls(testDoc);
            //result += getFiles(testDoc);
            return result;
        }

        // GET api/scraper/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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
    }
}

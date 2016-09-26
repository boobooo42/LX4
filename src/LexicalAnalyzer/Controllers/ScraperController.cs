using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Headers;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LexicalAnalyzer.Controllers
{
    [Route("api/[controller]")]
    public class ScraperController : Controller
    {
        static string getChildNodes(HtmlDocument doc)
        {
            string pureText = "";
            foreach (HtmlNode node in doc.DocumentNode.ChildNodes)
            {
                pureText += node.InnerText;
            }

            return pureText.ToString();
        }

        static string getLinks(HtmlDocument doc)
        {
            string urls = "";
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                urls += link.GetAttributeValue("href", string.Empty) + "\n";
            }
            return urls;
        }
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

        static async Task<string> RunAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://sourceforge.net/projects/pidgin/?source=directory");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                // HTTP GET
                HttpResponseMessage response = await client.GetAsync("/");
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("We did it, reddit!\n");
                    return await response.Content.ReadAsStringAsync();
                }
                return "it failed";
            }
        }

        // GET: api/scraper
        [HttpGet]
        public string Get()
        {
            Task<string> task = RunAsync();
            task.Wait();

            var testDoc = new HtmlDocument();
            testDoc.LoadHtml(task.Result);
            string result = "";
            //result = getChildNodes(testDoc);
            //result = getLinks(testDoc);
            result += getFiles(testDoc);
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

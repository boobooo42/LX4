using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using System.Net.Http;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LexicalAnalyzer.Controllers
{


    [Route("api/[controller]")]
    public class ScraperController : Controller
    {
        // GET: api/scraper
        [HttpGet]
        public IEnumerable<string> Get()
        {


            DownloadPageAsync();



            return new string[] { stringG };
        }

        // GET api/scraper/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        static string stringG;

        /// <summary>
        /// asynchronous call to an html doc
        /// </summary>
        async void DownloadPageAsync()
        {

            HtmlDocument testDoc = new HtmlDocument();
            HttpClient client = new HttpClient();

            //response gets the async response of the website AND the content
            var response = new HttpResponseMessage();
            response = await client.GetAsync("http://en.wikipedia.org/");

            var content = response.Content;
            string result = await content.ReadAsStringAsync();


            testDoc.LoadHtml(result);


            string urls = "";

            //this is just doing a node count for the document
            var root = testDoc.DocumentNode;
            var nodes = root.Descendants();
            var totalNodes = nodes.Count();

            foreach (HtmlNode link in testDoc.DocumentNode.SelectNodes("//a[@href]"))
            {
                urls += link.GetAttributeValue("href", string.Empty) + "\n";
            }


            stringG = urls;

        }

        //   return test;


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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;


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
            
            var testDoc = new HtmlDocument();
          //    HtmlWeb web = new HtmlWeb();
            testDoc.LoadHtml("http://stackoverflow.com/");
            //testDoc.Load()
            var root = testDoc.DocumentNode;
            var nodes = root.Descendants();
            var totalNodes = nodes.Count();
            string totes = totalNodes.ToString();
            string urls = "";
           // root.

            //foreach (HtmlNode link in testDoc. .DocumentNode.SelectNodes("//a[@href]"))
            //{
            //    urls += link.GetAttributeValue("href", string.Empty) + "\n";
            //}

            return new string[] { totes };
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

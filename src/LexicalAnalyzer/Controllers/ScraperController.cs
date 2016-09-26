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
        static async Task<string> RunAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://sourceforge.net");
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
            var testDoc = new HtmlDocument();
            //   HtmlWeb web = new HtmlWeb();
            //            testDoc.LoadHtml("http://www.stackoverflow.com");
            //            string urls = "";
            //foreach (HtmlNode link in testDoc.DocumentNode.SelectNodes("//a[@href]"))
            //{
            //    urls += link.GetAttributeValue("href", string.Empty) + "\n";
            //}

            Task<string> task = RunAsync();
            task.Wait();

            return task.Result;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LexicalAnalyzer.Controllers
{
    [Route("api/[controller]")]
    public class ScraperController : Controller
    {
        List<HtmlNode> DownLinkList;
        HtmlNodeCollection DownCollection;

        static string myURL = "http://debian.osuosl.org/debian/pool/main/c/c2050/";
        string DownloadPath = "//a[contains(@href,'.deb')]";
       // string DownloadPath = "//a[@href]";



        // GET: api/scraper
        [HttpGet]
        public string Get()
        {
    
            Task<string> task2 = TestContent(myURL);

            string binaries = GetBinaries(myURL);

            return binaries;
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

        public string GetBinaries(string URL)
        {
            var testDoc = new HtmlDocument();
            

            Task<string> task = RunBinaryAsync(URL);
 
            task.Wait();

            testDoc.LoadHtml(task.Result);
            string binaries = "";

            DownCollection = new HtmlNodeCollection(testDoc.DocumentNode);

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

        static async Task<string> TestContent(string URL)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "relativeAddress");
            //request.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}",
            //                                    Encoding.UTF8,
            //                                    "application/json");//CONTENT-TYPE header

            Console.WriteLine("Trying Connection\n");

            await client.SendAsync(request)
                  .ContinueWith(responseTask =>
                  {
                      Console.WriteLine("Response: {0}", responseTask.Result);
                      return "it worked\n";
                  });

            return "yay";
        }

        static async Task<string> RunBinaryAsync(string URL)
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

        static async Task<string> RunAsync(string URL)
        {


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Accept.Clear();
                //   client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
                // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                // HTTP GET
                HttpResponseMessage response = await client.GetAsync(URL);
                //  HttpContent content = response.Content;

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("We did it, reddit!\n");
                    return await response.Content.ReadAsStringAsync();
                }


                return "it failed";
            }
        }

    }
}

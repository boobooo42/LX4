using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LexicalAnalyzer.Controllers
{
    [Route("api/[controller]")]
    public class ShaController : Controller
    {
        // GET: api/sha/
        [HttpGet]
        public IEnumerable<string> Get()
        {
            SHA256 mySha = SHA256.Create();
            string testString = "Hi i'm just a test bro";
            //// Converts the string to bytes
            byte[] byteData = Encoding.UTF8.GetBytes(testString);
            //// hash data computed by SHA256
            byte[] hashData = mySha.ComputeHash(byteData);
            /// storing the SHA256 values in list
            List<string> hashList = new List<string>();
            string totalResults = " ";
            string results = " ";
            //// Converts the SHA256 to a readble format string of 2 pad 0
            int loopCount = 0;
            //// Loop to test list
            while(loopCount!=2)
            {
                results = " ";
                foreach (byte v in hashData)
                {
                    results = results + String.Format("{0:x2}", v);
                }
                hashList.Add(results);
                loopCount++;
            }
            /// Display list
            for (int count =0;count<hashList.Count;count++)
            {
                totalResults = totalResults + hashList[count];
            }
            return new string[] {"This is the test string: " +testString + "       Here is the results: "+ results+ "      Total :" +totalResults};
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
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

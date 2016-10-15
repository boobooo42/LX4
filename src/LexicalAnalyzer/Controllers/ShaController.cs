using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LexicalAnalyzer.Controllers
{
    [Route("api/[controller]")]
    public class ShaController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            string testString = "";
            SHA256 mySha = SHA256.Create();
            IFileRepository FileRepository = new FileRepository();
            List<File> files = FileRepository.GetAll();
            Database.Function.BlobMaker maker = new Database.Function.BlobMaker();
            maker.computeHash();
            foreach (File f in files)
            {
                
                testString = testString + "File content :" + f.FileContents +" File name: "+f.FileName +" File id: "+f.FileID+" File url: "+f.DownloadURL +
                    " date downloaded:" + f.DownloadDate +" hash: "+f.FileHash;
            }
            
            //
            /*
            //string testString = "Hi i'm just a test bro";
            //// Converts the string to bytes
            byte[] byteData = Encoding.UTF8.GetBytes(testString);
            //// hash data computed by SHA256
            byte[] hashData = mySha.ComputeHash(byteData);
            /// storing the SHA256 values in list
            List<string> hashList = new List<string>();
            string totalResults = " ";
            string results = " ";
            int dupCount = 0;
            //// loop to test
            int loopCount = 0;
            //// Converts the SHA256 to a readable format string of 2 pad 0
            while (loopCount != 3)
            {
                results = " ";
                foreach (byte v in hashData)
                {
                    results = results + String.Format("{0:x2}", v);
                }
                // checks dups
                if (hashList.Contains(results) != true)
                {
                    hashList.Add(results);
                }
                else
                {
                    dupCount++;
                }
                loopCount++;
            }
            /// Display list
            for (int count = 0; count < hashList.Count; count++)
            {
                totalResults = totalResults + hashList[count];
            }
            */
            return new string[] {testString};
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

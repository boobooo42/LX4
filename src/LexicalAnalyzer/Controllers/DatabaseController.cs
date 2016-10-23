using LexicalAnalyzer.Database;
using LexicalAnalyzer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace LexicalAnalyzer.Controllers
{
    public class DatabaseController : Controller
    {
        public DatabaseController() {
            /* TODO: Use dependency injection to get some handle into the
             * database */
        }

        [HttpGet("api/database/file")]
        public IEnumerable<string> GetFile()
        {
            return new string[] {
                "value2"
            };
        }

        [HttpPost("api/database/file")]
        public void PostFile()
        {
            File f = new File();
            f.FileContents = "foobar";
            f.FileID = 42;
            DatabaseTools.InsertIntoDatabase(f);
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

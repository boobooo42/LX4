using LexicalAnalyzer.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace LexicalAnalyzer.Controllers
{
    [Route("api/[controller]")]
    public class DatabaseController : Controller
    {
        IOptions<DatabaseOptions> m_options;
        public DatabaseController(IOptions<DatabaseOptions> options) {
            m_options = options;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] {
                m_options.Value.ConnectionString,
                "value2"
            };
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LexicalAnalyzer.Controllers
{
    public class CorpusController : Controller
    {
        [HttpGet("api/corpus/list")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}

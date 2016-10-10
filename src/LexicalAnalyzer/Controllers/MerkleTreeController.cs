using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LexicalAnalyzer.Controllers
{
    public class MerkleTreeController : Controller
    {
        // GET: api/values
        [HttpGet("api/merkletree/getnode")]
        public IEnumerable<string> GetNode(string hash)
        {
            return new string[] { "value1", "value2" };
        }
    }
}

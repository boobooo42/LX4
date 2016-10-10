using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LexicalAnalyzer.Controllers
{
    public class MerkleController : Controller
    {
        public ActionResult Index(){
            return View();
        }

        [HttpGet]
        public JsonResult Get()
        {
            var result = "Request: Get()";
            return Json(result);
        }

        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            var result = "Request: Get(int id)";
            return Json(result);
        }

        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace LexicalAnalyzer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About() 
        {
            ViewData["Result"] = "This is your about page ";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Status()
        {
            return View();
        }

        public IActionResult Documentation()
        {
            return View();
        }

        public IActionResult LearningModel()
        {
            return View();
        }

        public IActionResult Corpus()
        {
            return View();
        }
    }
}

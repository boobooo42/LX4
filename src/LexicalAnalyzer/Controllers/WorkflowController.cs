﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LexicalAnalyzer.Controllers
{
    public class WorkflowController : Controller
    {
        public IActionResult Manage()
        {
            return View();
        }

        public IActionResult Scraper()
        {
            return View();
        }

        public IActionResult TrainNet()
        {
            return View();
        }
        public IActionResult Learning()
        {
            return View();
        }
        public IActionResult ManageLearning()
        {
            return View();
        }
    }
}

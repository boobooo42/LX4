using HtmlAgilityPack;
using LexicalAnalyzer.Resources;
using LexicalAnalyzer.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace LexicalAnalyzer.Controllers
{
    public class ScraperController : Controller
    {
        // GET api/scraper/list
        [HttpGet("list")]
        public string List()
        {
            /* List all of the scrapers currently instantiated */
            return JsonConvert.SerializeObject(ScraperService.Scrapers);
        }

        // GET api/scraper/get
        [HttpGet("get")]
        public string Get(string guid)
        {
            /* Get a single scraper with the given guid */
            IScraper scraper = ScraperService.GetScraper(guid);
            return JsonConvert.SerializeObject(scraper);
        }

        // GET api/scraper/start
        [HttpGet("start")]
        public string Start(string guid)
        {
            ScraperService.StartScraper(guid);
            return "We're starting something!";
        }

        // GET api/scraper/pause
        [HttpGet("pause")]
        public string Pause(string guid)
        {
            ScraperService.PauseScraper(guid);
            return "We're pausing something!";
        }

        // GET api/scraper/types
        [HttpGet("types")]
        public List<string> Types()
        {
            return ScraperFactory.ScraperTypes;
        }

        // POST api/scraper/create
        [HttpPost("create")]
        public string Create(string type)
        {
            IScraper scraper = ScraperService.CreateScraper(type);
            return JsonConvert.SerializeObject(scraper);
        }

        // GET api/scraper
        [HttpGet("")]
        public string Default() {
            return "Hello world";
        }
    }
}

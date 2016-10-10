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
        [HttpGet("api/scraper/list")]
        public string List()
        {
            /* List all of the scrapers currently instantiated */
            return JsonConvert.SerializeObject(ScraperService.Scrapers);
        }

        // GET api/scraper/get
        [HttpGet("api/scraper/get")]
        public IScraper Get(string guid)
        {
            /* Get a single scraper with the given guid */
            IScraper scraper = ScraperService.GetScraper(guid);
            return scraper;
//            return JsonConvert.SerializeObject(scraper);
        }

        // GET api/scraper/start
        [HttpGet("api/scraper/start")]
        public string Start(string guid)
        {
            ScraperService.StartScraper(guid);
            return "We're starting something!";
        }

        // GET api/scraper/pause
        /// <summary>
        /// Pauses the scraper task with the given GUID, if it exists.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This method pauses the scraper task with the given GUID. The net
        /// effect is that if the scraper task is currently running, a pause
        /// command is sent to the Worker object that is running the task. If
        /// the scraper task is not running, it is removed from the task queue.
        /// </p>
        /// <p>
        /// If the given GUID does not correspond to any known scraper tasks,
        /// then this method does nothing.
        /// </p>
        /// </remarks>
        /// <param name="guid">GUID of the scraper task to pause</param>
        /// <returns></returns>
        [HttpGet("api/scraper/pause")]
        public string Pause(string guid)
        {
            ScraperService.PauseScraper(guid);
            return "We're pausing something!";
        }

        // GET api/scraper/types
        /// <summary>
        /// Returns a list of all of the scraper types supported.
        /// </summary>
        /// <remarks>
        /// The scraper types returned here are the fully qualified names of
        /// all of the classes implementing IScraper that have been loaded by
        /// ScraperFactory. Scrapers can be created using any of these types.
        /// </remarks>
        [HttpGet("api/scraper/types")]
        public List<string> Types()
        {
            /* TODO: Return more info, such as the description of each type */
            return ScraperFactory.ScraperTypes;
        }

        // POST api/scraper/create
        [HttpPost("api/scraper/create")]
        public string Create(string type)
        {
            IScraper scraper = ScraperService.CreateScraper(type);
            return JsonConvert.SerializeObject(scraper);
        }
    }
}

using HtmlAgilityPack;
using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using LexicalAnalyzer.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System;

namespace LexicalAnalyzer.Controllers
{
    public class ScraperController : Controller
    {
        /* Private members */
        IScraperService m_scraperService;
        IScraperFactory m_scraperFactory;

        /* Constructors */
        public ScraperController(
                IScraperService scraperService,
                IScraperFactory scraperFactory)
        {
            m_scraperService = scraperService;
            m_scraperFactory = scraperFactory;
        }

        /// <summary>
        /// Returns a list of all of the scrapers currently instantiated.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This method returns a list of all of the scrapers currently
        /// instantiated, as currently known by the ScraperSubsystem.
        /// </p>
        /// </remarks>
        [HttpGet("api/scraper")]
        public string List()
        {
            /* List all of the scrapers currently instantiated */
            return JsonConvert.SerializeObject(m_scraperService.Scrapers);
        }

        public class SerializeStatusContractResolver : DefaultContractResolver
        {
            public new static readonly SerializeStatusContractResolver Instance
                = new SerializeStatusContractResolver();

            protected override JsonProperty CreateProperty(
                    MemberInfo member,
                    MemberSerialization memberSerialization)
            {
                JsonProperty property =
                    base.CreateProperty(
                            member,
                            memberSerialization);

                if (property.DeclaringType
                        .GetInterfaces()
                        .Contains(typeof(IScraper)))
                {
                    if (property.PropertyName == "Progress")
                    {
                        property.ShouldSerialize =
                            instance =>
                            {
                                IScraper scraper = (IScraper)instance;
                                return (scraper.Status == "running")
                                    || (scraper.Status == "paused");
                            };
                    }
                    else if (property.PropertyName == "Status")
                    {
                        property.ShouldSerialize =
                            instance => { return true; };
                    }
                    else if (property.PropertyName == "Guid")
                    {
                        property.ShouldSerialize =
                            instance => { return true; };
                    }
                    else
                    {
                        property.ShouldSerialize =
                            instance => { return false; };
                    }
                }

                return property;
            }
        }

        /// <summary>
        /// Returns the status and progress of all of the scrapers currently
        /// instantiated.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This method returns a list of all of the scrapers currently
        /// instantiated, along with each of their status and progress
        /// attributes. This allows the caller to check on the most recent
        /// status of all scrapers without needing to download all scraper
        /// attributes.
        /// </p>
        /// </remarks>
        [HttpGet("api/scraper/status")]
        public string Status()
        {
            return JsonConvert.SerializeObject(
                    m_scraperService.Scrapers,
                    Formatting.Indented,
                    new JsonSerializerSettings {
                    ContractResolver = SerializeStatusContractResolver.Instance
                    });
        }

        /// <summary>
        /// Returns scraper instance with the given GUID
        /// </summary>
        /// <remarks>
        /// <p>
        /// This method returns the scraper instance with the given GUID, if
        /// such a scraper instance exists.
        /// </p>
        /// </remarks>
        [HttpGet("api/scraper/{guid}")]
        public string Get(string guid)
        {
            IScraper scraper = m_scraperService.GetScraper(guid);
            if (scraper == null) {
                var error = new LexicalAnalyzer.Models.Error();
                error.Message = "Could not find Scraper with the given GUID";
                return JsonConvert.SerializeObject(error);
            }
            return JsonConvert.SerializeObject(scraper);
        }

        /// <summary>
        /// Deletes the scraper instance with the given GUID
        /// </summary>
        /// <remarks>
        /// <p>
        /// This method removes the scraper instance with the given GUID, if
        /// such a scraper instance exists. If the scraper task is currently
        /// running, it is killed immediately.
        /// </p>
        /// <p>
        /// If no scraper instance with the given GUID exists, then this call
        /// does nothing.
        /// </p>
        /// </remarks>
        [HttpDelete("api/scraper/{guid}")]
        public void Delete(string guid)
        {
            /* Remove the scraper with the given guid */
            /* TODO: Return success or failure status ? */
            m_scraperService.RemoveScraper(guid);
        }

        /// <summary>
        /// Starts the scraper task with the given GUID
        /// </summary>
        /// <remarks>
        /// <p>
        /// This method starts the scraper task with the given GUID. If the
        /// scraper with the given GUID is not yet running, it is added to the
        /// scraper task queue. Depending on the relative priority of this
        /// scraper task, it is run as soon as a task worker becomes available.
        /// </p>
        /// <p>
        /// If the scraper task with the given GUID has already been added to
        /// the scraper task queue, then this method has no effect.
        /// </p>
        /// <p>
        /// If the given GUID does not correspond to any known scraper tasks,
        /// then this method does nothing.
        /// </p>
        /// </remarks>
        [HttpPost("api/scraper/{guid}/start")]
        public string Start(string guid)
        {
            m_scraperService.StartScraper(guid);
            return "We're starting something!";
        }

        /// <summary>
        /// Start all scraper tasks.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This method starts all scraper tasks that have not yet been
        /// started. If there are not enough worker threads to start all
        /// scraper tasks immediately, then the remaining tasks are queued up
        /// </p>
        /// <p>
        /// If all scraper tasks have already been started, then this call has
        /// no effect.
        /// </p>
        /// </remarks>
        [HttpPost("api/scraper/start")]
        public void StartAll()
        {
            foreach (IScraper scraper in m_scraperService.Scrapers) {
                m_scraperService.StartScraper(scraper.Guid);
            }
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
        [HttpPost("api/scraper/{guid}/pause")]
        public string Pause(string guid)
        {
            /* FIXME: Check for null guid values */
            m_scraperService.PauseScraper(guid);
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
        public IEnumerable<ScraperType> Types()
        {
            return m_scraperFactory.ScraperTypes;
        }

        /// <summary>
        /// Creates a scraper instance of the given type.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This call creates a scraper instance of the given type. The types
        /// available can be found through the /api/scraper/types call.
        /// </p>
        /// <p>
        /// Created scrapers are initially idle and must be started through a
        /// call to /api/scraper/{guid}/start.
        /// </p>
        /// </remarks>
        [HttpPost("api/scraper/{type}")]
        public string Create(string type, [FromBody] Scraper s)
        {
            if (s == null) {
                /* The JSON sent was not in the correct format */
                Response.StatusCode = 400;  /* Bad Request */
                var error = new LexicalAnalyzer.Models.Error();
                error.Message = "Invalid structure for Scraper object";
                return JsonConvert.SerializeObject(error);
            }
            if (s.Status != "init") {
                var error = new LexicalAnalyzer.Models.Error();
                error.Message = "Initial Scraper status must be 'init'";
                return JsonConvert.SerializeObject(error);
            }
            IScraper scraper = m_scraperService.CreateScraper(type);
            if (scraper == null) {
                var error = new LexicalAnalyzer.Models.Error();
                error.Message = String.Format(
                        "Unknown scraper type '{0}'", type);
                return JsonConvert.SerializeObject(error);
            }
            scraper.Status = s.Status;
            scraper.Properties = s.Properties;
            return JsonConvert.SerializeObject(scraper);
        }

        /// <summary>
        /// Set parameters of a scraper
        /// </summary>
        /// <remarks>
        /// <p>
        /// </p>
        /// </remarks>
        [HttpPut("api/scraper/{guid}")]
        public string Update(string guid, [FromBody] Scraper s)
        {
            IScraper scraper = m_scraperService.GetScraper(guid);
            if (scraper == null) {
                var error = new LexicalAnalyzer.Models.Error();
                error.Message = "Could not find Scraper with the given GUID";
                return JsonConvert.SerializeObject(error);
            }
            if (s == null) {
                /* The JSON sent was not in the correct format */
                Response.StatusCode = 400;  /* Bad Request */
                var error = new LexicalAnalyzer.Models.Error();
                error.Message = "Invalid structure for Scraper object";
                return JsonConvert.SerializeObject(error);
            }
            scraper.Properties = s.Properties;
            if (s.Status.ToLower() == "start") {
                m_scraperService.StartScraper(guid);
            } else if (s.Status.ToLower() == "pause") {
                m_scraperService.PauseScraper(guid);
            } else {
                Response.StatusCode = 400;  /* Bad Request */
                var error = new LexicalAnalyzer.Models.Error();
                error.Message = "The only valid Scraper status values to set are 'start' or 'pause'";
                return JsonConvert.SerializeObject(error);
            }
            return JsonConvert.SerializeObject(scraper);
        }

        /*
        // PATCH api/scraper/{guid}
        /// <summary>
        /// Set some of the parametrs of a scraper
        /// </summary>
        /// <remarks>
        /// <p>
        /// Certain parameters of the a scraper can be updated using this call.
        /// Note that some aspects of the scraper, such as the GUID or display
        /// name, cannot be modified.
        /// </p>
        /// </remarks>
        [HttpPatch("api/scraper/{guid}")]
        public string Create()
        {
            return "";
        }
        */
    }
}

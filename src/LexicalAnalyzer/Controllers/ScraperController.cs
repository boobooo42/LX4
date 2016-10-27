using HtmlAgilityPack;
using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System;

namespace LexicalAnalyzer.Controllers
{
    public class ScraperController : Controller
    {
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
            return JsonConvert.SerializeObject(ScraperService.Scrapers);
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
                    ScraperService.Scrapers,
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
        public IScraper Get(string guid)
        {
            if (guid == null) {
                return null;
            }
            /* Get a single scraper with the given guid */
            IScraper scraper = ScraperService.GetScraper(guid);
            return scraper;
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
            ScraperService.RemoveScraper(guid);
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
            ScraperService.StartScraper(guid);
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
            foreach (IScraper scraper in ScraperService.Scrapers) {
                ScraperService.StartScraper(scraper.Guid);
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
        [HttpPost("api/scraper")]
        public string Create([FromBody] string type)
        {
            /* FIXME: Check for null (or invalid?) type values? */
            IScraper scraper = ScraperService.CreateScraper(type);
            Debug.WriteLine("scraper type requested: " + type);
            return JsonConvert.SerializeObject(scraper);
        }

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
            /* TODO: I have no idea what to do here */
            return "";
        }
    }
}

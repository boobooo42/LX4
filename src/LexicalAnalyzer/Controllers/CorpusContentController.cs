using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Data.SqlTypes;
using System.Text;
using Dto;
using LexicalAnalyzer.Scrapers;

namespace LexicalAnalyzer.Controllers
{
    [Route("api/[controller]")]
    public class CorpusContentController : Controller
    {
        /* Private members */
        ICorpusContext m_context;

        /* Constructors */
        public CorpusContentController(ICorpusContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Get a list of corpora stored on the database
        /// </summary>
        [HttpGet("list/{id}")]
        public IEnumerable<CorpusContent> List(int id)
        { 
            return m_context.CorpusContentRepository.List(id);
        }

        /// <summary>
        /// Delete a Corpus Content of the given id
        /// </summary>
        [HttpGet("delete/{id}")]
        public void Delete(int id)
        {
            var temp = m_context.CorpusContentRepository.GetById(id);
            m_context.CorpusContentRepository.Delete(temp);
        }

        /// <summary>
        /// Adds content to a given corpus.
        /// </summary>
        /// <param name="obj"></param>
        [HttpPost("add")]
        public String Add([FromBody] CorpusContentDto obj)
        {
            var content = new CorpusContent();

            try
            {
                content = new CorpusContent()
                {
                    Id = -1,
                    CorpusId = obj.CorpusId,
                    Name = obj.Name,
                    Content = Encoding.ASCII.GetBytes(obj.Content),
                    Type = obj.Type,
                    Hash = ScraperUtilities._hashContent(Encoding.ASCII.GetBytes(obj.Content)),
                    DownloadDate = DateTime.Now,
                    ScraperType = "Manual Insert",
                };

                m_context.CorpusContentRepository.Add(content);
            }
            catch (Exception e)
            {
                return e.ToString();
            }

            return "All Good";
        }
    }
}
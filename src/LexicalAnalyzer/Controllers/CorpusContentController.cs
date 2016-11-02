using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Data.SqlTypes;
using Dto;

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
        /// Get a list of corpora stored on the database
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
        public void Add([FromBody] CorpusContentDto obj)
        {
            var corpusContent = new CorpusContent()
            {
                Id = obj.Id,
                CorpusId = obj.CorpusId,
                Content = obj.Content,
                DownloadDate = DateTime.Now,
                DownloadURL = "N/A",
                Name = obj.Name,
                Hash = "Not Assigned.",
                ScraperGuid = new Guid(),
                ScraperType = "Manual Insert",
                Type = obj.Type
            };

            m_context.CorpusContentRepository.Add(corpusContent);
        }
    }
}
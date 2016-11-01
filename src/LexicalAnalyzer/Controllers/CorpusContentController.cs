using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Data.SqlTypes;

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
        [HttpGet("api/content/delete")]
        public void Delete(int id)
        {
            var temp = m_context.CorpusContentRepository.GetById(id);
            m_context.CorpusContentRepository.Delete(temp);
        }

        /// <summary>
        /// Adds content to a given corpus.
        /// </summary>
        /// <param name="obj"></param>
        [HttpPost("api/content/add")]
        public void Add([FromBody] object obj)
        {
            var m = Json(obj);
            var a = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var corpusContent = (CorpusContent)Newtonsoft.Json.JsonConvert.DeserializeObject(a, typeof(CorpusContent));
            corpusContent.ScraperGuid = new SqlGuid(Guid.NewGuid().ToString());
            corpusContent.Hash = Guid.NewGuid().ToString();
            corpusContent.Id = 43;
            m_context.CorpusContentRepository.Add(corpusContent);
        }
    }
}
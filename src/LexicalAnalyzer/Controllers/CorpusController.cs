using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace LexicalAnalyzer.Controllers
{
    public class CorpusController : Controller
    {
        /* Private members */
        ICorpusContext m_context;

        /* Constructors */
        public CorpusController(ICorpusContext context) {
            m_context = context;
        }

        /// <summary>
        /// Get a list of corpora stored on the database
        /// </summary>
        [HttpGet("api/corpus")]
        public IEnumerable<Corpus> List()
        {
            return m_context.CorpusRepository.List();
        }
    }
}

using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace LexicalAnalyzer.Controllers {
    public class LearningModelController : Controller {
        /* Private members */
        ILearningModelService m_learningService;
        ILearningModelFactory m_learningFactory;

        /* Constructors */
        public LearningModelController(
                ILearningModelService learningService,
                ILearningModelFactory learningFactory)
        {
            m_learningService = learningService;
            m_learningFactory = learningFactory;
        }

        /// <summary>
        /// Returns a list of all of the learning model types supported.
        /// </summary>
        /// <remarks>
        /// <p>
        /// The objects returned here represent all of the classes implementing
        /// the ILearningModel interface that have been loaded by
        /// LearningModelFactory. 
        /// </p>
        /// <p>
        /// The name property is the fully qualified name of a class
        /// implementing ILearningModel. This string can be used in the POST
        /// /api/learningModel/{type} call to construct a learning model of
        /// that type.
        /// </p>
        /// </remarks>
        [HttpGet("api/learningModel/types")]
        public IEnumerable<LearningModelType> Types()
        {
            return m_learningFactory.LearningModelTypes;
        }
    }
}

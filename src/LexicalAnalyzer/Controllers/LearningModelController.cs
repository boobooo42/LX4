using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        [HttpGet("api/learningmodel/types")]
        public IEnumerable<LearningModelType> Types()
        {
            return m_learningFactory.LearningModelTypes;
        }

        /// <summary>
        /// Creates a learning model instance of the given type.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This call creates a learning model instance of the given type. The
        /// types available can be found through the /api/learningmodel/types call.
        /// </p>
        /// <p>
        /// Learning models must be given an initial status value of "init".
        /// Created learning models are initially idle and must be started by
        /// changing this status value to "start" through a call to PUT
        /// /api/scraper/{guid}.
        /// </p>
        /// </remarks>
        [HttpPost("api/learningmodel/{type}")]
        public string Create(string type, [FromBody] LearningModel lm)
        {
            if (lm == null) {
                /* The JSON sent was not in the correct format */
                Response.StatusCode = 400;  /* Bad Request */
                var error = new LexicalAnalyzer.Models.Error();
                error.Message = "Invalid structure for LearningModel object";
                return JsonConvert.SerializeObject(error);
            }
            if (lm.Status != "init") {
                var error = new LexicalAnalyzer.Models.Error();
                error.Message = "Initial LearningModel status must be 'init'";
                return JsonConvert.SerializeObject(error);
            }
            ILearningModel learningModel =
                m_learningService.CreateLearningModel(type);
            if (learningModel == null) {
                var error = new LexicalAnalyzer.Models.Error();
                error.Message = string.Format(
                        "Unknown learning model type '{0}'", type);
                return JsonConvert.SerializeObject(error);
            }
            learningModel.Status = lm.Status;
            learningModel.Properties = lm.Properties;
            return JsonConvert.SerializeObject(learningModel);
        }

        /// <summary>
        /// Returns a list of all of the learning models currently
        /// instantiated.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This method returns a list of all of the scrapers currently
        /// instantiated, as currently known by the LearningModelSubsystem.
        /// </p>
        /// </remarks>
        [HttpGet("api/learningmodel")]
        public string List()
        {
            /* List all learning models currently instantiated */
            return JsonConvert.SerializeObject(
                    m_learningService.LearningModels);
        }

        /// <summary>
        /// Update the learning model with the given GUID.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This call is most useful for changing the status of any given
        /// learning model. Learning models can be started or paused by setting
        /// the status value to "start" or "pause" respectively.
        /// </p>
        /// </remarks>
        [HttpPut("api/learningmodel/{guid}")]
        public string Update(string guid, [FromBody] LearningModel lm)
        {
            ILearningModel learningModel =
                m_learningService.GetLearningModel(guid);
            if (learningModel == null) {
                Response.StatusCode = 404;  /* Not Found */
                var error = new LexicalAnalyzer.Models.Error();
                error.Message =
                    "Could not find learning model with the given GUID";
                return JsonConvert.SerializeObject(error);
            }
            if (lm == null) {
                /* The JSON sent was not in the correct format */
                Response.StatusCode = 400;  /* Bad Request */
                var error = new LexicalAnalyzer.Models.Error();
                error.Message = "Invalid structure for learning model object";
                return JsonConvert.SerializeObject(error);
            }
            learningModel.Properties = lm.Properties;
            if (lm.Status.ToLower() == "start") {
                m_learningService.StartLearningModel(guid);
            } else if (lm.Status.ToLower() == "pause") {
                m_learningService.PauseLearningModel(guid);
            } else {
                Response.StatusCode = 400;  /* Bad Request */
                var error = new LexicalAnalyzer.Models.Error();
                error.Message = "The only valid learning model status values to set are 'start' or 'pause'";
                return JsonConvert.SerializeObject(error);
            }
            return JsonConvert.SerializeObject(learningModel);
        }

        /*
        [HttpGet("api/learningmodel/results/{guid}")]
        public string Results(string guid) {
            return JsonConvert.SerializeObject();
        }
        */
    }
}

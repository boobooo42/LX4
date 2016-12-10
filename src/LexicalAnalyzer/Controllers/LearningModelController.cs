using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System;

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
        /// /api/learningmodel/{guid}.
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

        public class SerializeLearningModelContractResolver
            : DefaultContractResolver
        {
            public static readonly SerializeLearningModelContractResolver
                Instance = new SerializeLearningModelContractResolver();

            protected override JsonProperty CreateProperty(
                    MemberInfo member,
                    MemberSerialization memberSerialization)
            {
                JsonProperty property =
                    base.CreateProperty(
                            member,
                            memberSerialization);

                /* Suppress the serialization of the Result proprety of the
                 * learning model, since it is expensive to serialize */
                if (property.DeclaringType
                        .GetInterfaces()
                        .Contains(typeof(ILearningModel)))
                {
                    if (property.PropertyName == "Result") {
                        property.ShouldSerialize =
                            instance => { return false; };
                    }
                }

                return property;
            }
        }

        /// <summary>
        /// Returns a list of all of the learning models currently
        /// instantiated.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This method returns a list of all of the learning models currently
        /// instantiated, as currently known by the LearningModelSubsystem.
        /// </p>
        /// </remarks>
        [HttpGet("api/learningmodel")]
        public string List()
        {
            /* List all learning models currently instantiated */
            return JsonConvert.SerializeObject(
                    m_learningService.LearningModels,
                    Formatting.Indented,
                    new JsonSerializerSettings {
                    ContractResolver = SerializeLearningModelContractResolver.Instance
                    });
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

        /// <summary>
        /// Deletes the learning model instance with the given GUID
        /// </summary>
        /// <remarks>
        /// <p>
        /// This method removes the learning model instance with the given GUID, if
        /// such a learning model instance exists. If the learning model task is currently
        /// running, it is killed immediately.
        /// </p>
        /// <p>
        /// If no learning model instance with the given GUID exists, then this call
        /// does nothing.
        /// </p>
        /// </remarks>
        [HttpDelete("api/learningmodel/{guid}")]
        public void Delete(string guid)
        {
            /* Remove the learning model with the given guid */
            /* TODO: Return success or failure status ? */
            m_learningService.RemoveLearningModel(guid);
        }

        /// <summary>
        /// Starts the learning model task with the given GUID
        /// </summary>
        /// <remarks>
        /// <p>
        /// This method starts the learning model task with the given GUID. If the
        /// learning model with the given GUID is not yet running, it is added to the
        /// learning model task queue. Depending on the relative priority of this
        /// learning model task, it is run as soon as a task worker becomes available.
        /// </p>
        /// <p>
        /// If the learning model task with the given GUID has already been added to
        /// the learning model task queue, then this method has no effect.
        /// </p>
        /// <p>
        /// If the given GUID does not correspond to any known learning model tasks,
        /// then this method does nothing.
        /// </p>
        /// </remarks>
        [HttpPost("api/learningmodel/{guid}/start")]
        public string Start(string guid)
        {
            m_learningService.StartLearningModel(guid);
            return "We're starting something!";
        }

        /// <summary>
        /// Start all learning model tasks.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This method starts all learning model tasks that have not yet been
        /// started. If there are not enough worker threads to start all
        /// learning model tasks immediately, then the remaining tasks are queued up
        /// </p>
        /// <p>
        /// If all learning model tasks have already been started, then this call has
        /// no effect.
        /// </p>
        /// </remarks>
        [HttpPost("api/learningmodel/start")]
        public void StartAll()
        {
            foreach (ILearningModel learn in m_learningService.LearningModels)
            {
                m_learningService.StartLearningModel(learn.Guid);
            }
        }

        // GET api/learningmodel/pause
        /// <summary>
        /// Pauses the learning model task with the given GUID, if it exists.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This method pauses the learning model task with the given GUID. The net
        /// effect is that if the learning model task is currently running, a pause
        /// command is sent to the Worker object that is running the task. If
        /// the learning model task is not running, it is removed from the task queue.
        /// </p>
        /// <p>
        /// If the given GUID does not correspond to any known learning model tasks,
        /// then this method does nothing.
        /// </p>
        /// </remarks>
        /// <param name="guid">GUID of the learning model task to pause</param>
        /// <returns></returns>
        [HttpPost("api/learningmodel/{guid}/pause")]
        public string Pause(string guid)
        {
            /* FIXME: Check for null guid values */
            m_learningService.PauseLearningModel(guid);
            return "We're pausing something!";
        }

        public class RawJsonConverter : JsonConverter
        {
            public override bool CanConvert(
                    Type objectType)
            {
                if (objectType == typeof(string))
                    return true;
                return false;
            }

            public override void WriteJson(
                    JsonWriter writer,
                    Object value,
                    JsonSerializer serializer)
            {
                Debug.Assert(value.GetType() == typeof(string));
                writer.WriteRaw((string)value);
            }

            public override Object ReadJson(
                    JsonReader reader,
                    Type objectType,
                    Object existingValue,
                    JsonSerializer serializer)
            {
                Debug.Assert(false);
                return null;
            }
        }

        public class ResultConverter : JsonConverter
        {
            public override bool CanConvert(
                    Type objectType)
            {
                if (objectType.GetInterfaces().Contains(typeof(IResult)))
                    return true;
                return false;
            }

            public override void WriteJson(
                    JsonWriter writer,
                    Object value,
                    JsonSerializer serializer)
            {
                Debug.Assert(value.GetType()
                        .GetInterfaces()
                        .Contains(typeof(IResult)));
                var result = (IResult)value;
                /* TODO: Write the result type as JSON */
                writer.WriteStartObject();
                writer.WritePropertyName("type");
                writer.WriteValue(result.Type);
                writer.WritePropertyName("data");
                writer.WriteRawValue(result.Data);
                writer.WriteEndObject();
            }

            public override Object ReadJson(
                    JsonReader reader,
                    Type objectType,
                    Object existingValue,
                    JsonSerializer serializer)
            {
                Debug.Assert(false);
                return null;
            }
        }

        [HttpGet("api/learningmodel/{guid}/result")]
        public string Result(string guid) {
            ILearningModel learningModel =
                m_learningService.GetLearningModel(guid);
            if (learningModel == null) {
                Response.StatusCode = 404;  /* Not Found */
                var error = new LexicalAnalyzer.Models.Error();
                error.Message =
                    "Could not find learning model with the given GUID";
                return JsonConvert.SerializeObject(error);
            }
            return JsonConvert.SerializeObject(
                    learningModel.Result,
                    new ResultConverter()
                    );
        }
    }
}

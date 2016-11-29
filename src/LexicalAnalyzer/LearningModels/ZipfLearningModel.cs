using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using System.Collections.Generic;
using System;

namespace LexicalAnalyzer.LearningModels {
    public class ZipfLearningModel : ILearningModel {
        /* Private data members */
        private Guid m_guid;
        private ICorpusContext m_context;

        /* Constructors */
        public ZipfLearningModel(ICorpusContext context) {
            m_guid = System.Guid.NewGuid();
            // this.Status = "init";  /* FIXME */
            m_context = context;
        }

        /* Public static interface */
        /// <summary>
        /// Static method returning the display name string for the Zipf's Law
        /// learning model.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This static method is required by LearningModelFactory for all
        /// learning models.
        /// </p>
        /// </remarks>
        public static string DisplayName {
            get { return "Zipf's Law"; }
        }

        /// <summary>
        /// Static method returning the human-readable description of the
        /// Zipf's Law learning model.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This static method is required by LearningModelFactory for all
        /// learning models.
        /// </p>
        /// </remarks>
        public static string Description {
            get {
                return
                    @"A learning model that demonstrate's Zipf's law of the
                    logrithmic relationship between rank and frequency of
                    words/letters.";
            }
        }

        /// <summary>
        /// Static method returning a list of the default properties for the
        /// Zipf's Law learning model.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This static method is required by LearningModelFactory for all
        /// learning models.
        /// </p>
        /// <p>
        /// Note that the ZIpf's Law learning model does not expose any
        /// properties at this time.
        /// </p>
        /// </remarks>
        public static IEnumerable<KeyValueProperty> DefaultProperties {
            get {
                var properties = new List<KeyValueProperty>();
                /* TODO: Add properties (if they are even needed) */
                return properties;
            }
        }

        /* Public interface */
        public string Type
        {
            get
            {
                return this.GetType().FullName;
            }
        }

        /// <summary>
        /// The GUID for this learning model.
        /// </summary>
        /// <remarks>
        /// This GUID is unique for this object and is created at construction
        /// time.
        /// </remarks>
        public Guid Guid {
            get {
                return m_guid;
            }
        }


        /// <summary>
        /// Status of the learning model task, implementing method from ITask.
        /// </summary>
        public string Status {
            /* TODO? */
            get; set;
        }

        /// <summary>
        /// Progress of the learning model task, implementing method from
        /// ITask.
        /// </summary>
        public float Progress {
            /* TODO */
            get { return 0.0f; }
        }

        /// <summary>
        /// Priority of this learning model task, implementing method from
        /// ITask.
        /// </summary>
        public int Priority {
            /* TODO */
            get { return 0; }
        }

        public IEnumerable<KeyValueProperty> Properties {
            get;
            set;
        } = DefaultProperties;

        public void Run() {
            /* TODO: Read from the corpus */
            /* TODO: Calculate the frequency/rank of words/letters */
        }
    }
}
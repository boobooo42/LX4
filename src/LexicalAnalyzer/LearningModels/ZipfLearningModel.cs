using LexicalAnalyzer.Exceptions;
using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using System.Collections.Generic;
using System;

namespace LexicalAnalyzer.LearningModels {
    public class ZipfLearningModel : ILearningModel {
        /* Private data members */
        private Guid m_guid;
        private IMerkleTreeContext m_context;
        private List<KeyValueProperty> m_properties;
        private long m_corpusID;

        /* Constructors */
        public ZipfLearningModel(IMerkleTreeContext context) {
            m_guid = System.Guid.NewGuid();
            m_context = context;
            // this.Status = "init";  /* FIXME */
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
                properties.Add(new KeyValueProperty(
                            "corpus",  /* Key */
                            "",  /* DefaultValue */
                            "corpus_id"  /* Type */
                            ));
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
            get {
                return m_properties;
            }
            set {
                foreach (var property in value) {
                    switch (property.Key.ToLower()) {
                        case "corpus":
                            /* FIXME: Check if this is a valid long */
                            m_corpusID = Convert.ToInt64(property.Value);
                            break;
                        default:
                            throw new LearningModelException(String.Format(
                                "ZipfLearningModel does not recognize option '{1}'",
                                property.Key));
                    }
                }
                m_properties = new List<KeyValueProperty>(value);
            }
        }

        public void Run() {
            /* TODO: Read from the corpus */
            var corpusBlob = m_context.CorpusBlobRepository
                .GetByCorpusID(m_corpusID);

            /* TODO: Iterate over all corpus content */
            foreach (var content in corpusBlob.Content) {
            }

            /* TODO: Calculate the frequency/rank of words/letters */
        }

        public IResult Result {
            get {
                /* XXX: Replace this with real data */
                var words = new List<RankFrequencyPair>();
                words.Add(new RankFrequencyPair(
                            "foo",  /* name */
                            1,  /* rank */
                            2935  /* frequency */
                            ));
                words.Add(new RankFrequencyPair(
                            "bar",  /* name */
                            2,  /* rank */
                            235  /* frequency */
                            ));
                words.Add(new RankFrequencyPair(
                            "baz",  /* name */
                            3,  /* rank */
                            20  /* frequency */
                            ));
                var characters = new List<RankFrequencyPair>();
                ZipfResult result = new ZipfResult(
                    words,  /* words */
                    characters  /* characters */
                    );
                return result;
            }
        }
    }
}

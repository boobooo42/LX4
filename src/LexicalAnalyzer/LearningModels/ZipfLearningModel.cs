using LexicalAnalyzer.Exceptions;
using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;

namespace LexicalAnalyzer.LearningModels {
    public class ZipfLearningModel : ILearningModel {
        /* Private data members */
        private Guid m_guid;
        private IMerkleTreeContext m_context;
        private List<KeyValueProperty> m_properties;
        private long m_corpusID;
        private List<RankFrequencyPair> m_words;

        /* Constructors */
        public ZipfLearningModel(IMerkleTreeContext context) {
            m_guid = System.Guid.NewGuid();
            m_context = context;
            // this.Status = "init";  /* FIXME */
            m_words = new List<RankFrequencyPair>();
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
            /* Read from the corpus */
            var corpusBlob = m_context.CorpusBlobRepository
                .GetByCorpusID(m_corpusID);

            /* Iterate over all corpus content */
            var dictionary = new SortedDictionary<string, RankFrequencyPair>();
            foreach (var cont in corpusBlob.Content) {
                /* Note that we only have the content hash at this point. We
                 * must retrieve the actual content from the
                 * ContentBlobRepository. */
                var content =
                    m_context.ContentBlobRepository.GetByHash(cont.Hash);
                var text = content.Contents;
                /* TODO: We split the text into words and compute the frequency
                 * of each */
                /* From: http://stackoverflow.com/a/16734675 */
                var punctuation = text.Where(Char.IsPunctuation).Distinct().ToArray();
                var words = text.Split().Select(x => x.Trim(punctuation));
                foreach (var word in words) {
                    if (!dictionary.ContainsKey(word)) {
                        dictionary.Add(word, new RankFrequencyPair(
                                    word,  /* name */
                                    -1,  /* rank */
                                    0  /* frequency */
                                    ));
                    }
                    dictionary[word].Frequency += 1;
                }
            }
            /* We sort the words by frequency to determine the rank of each
             * word */
            var rankedWords = dictionary.Values.OrderBy(word => word.Frequency);
            for (int i = 0; i < rankedWords.Count(); ++i) {
                rankedWords.ElementAt(i).Rank = i + 1;
            }

            /* TODO: Implement thread safety here */
            m_words = new List<RankFrequencyPair>(rankedWords);
        }

        public IResult Result {
            get {
                /* FIXME: Implement thread safety here */
                var result = new ZipfResult(
                        m_words,  /* words */
                        new List<RankFrequencyPair>()  /* characters */
                        );
                return result;
            }
        }
    }
}

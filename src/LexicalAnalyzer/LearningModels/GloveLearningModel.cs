using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using LexicalAnalyzer.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace LexicalAnalyzer.Scrapers
{
    public class CooccurArgs {
        int verbose, symmetric, windowSize;
        float memory;
        int maxProduct, overflowLength;
        string overflowFile;
        int mode;
        public CooccurArgs() {
            verbose = 0;
            symmetric = 1;
            windowSize = 15;
            memory = 4;
            maxProduct = -1;
            overflowLength = -1;
            overflowFile = "overflow";
            mode = 0;
        }
    }
    public class GloveArgs {
        int verbose, vectorSize, threads, iter;
        float eta, alpha, xMax;
        int binary, model;
        int saveGradsq, checkpointEvery, mode;
        public GloveArgs() {
            verbose = 0;
            vectorSize = 50;
            threads = 8;
            iter = 25;
            eta = 0.05f;
            alpha = 0.75f;
            xMax = 100.0f;
            binary = 0;
            model = 2;
            saveGradsq = 0;
            checkpointEvery = 0;
            mode = 0;
        }
    }
    public class ShuffleArgs {
        int verbose;
        float memory;
        int arraySize;
        string tempFile;
        int mode;
        ShuffleArgs() {
            verbose = 0;
            memory = 4.0f;
            arraySize = -1;
            tempFile = "temp_shuffle";
            mode = 0;
        }
    }
    public class VocabCountArgs {
        int verbose, maxVocab, minCount, mode;
        VocabCountArgs() {
            verbose = 0;
            maxVocab = -1;
            minCount = 1;
            mode = 0;
        }
    }

    public class GloveLearningModel : ILearningModel
    {
        private ICorpusContext m_context;

        public GloveLearningModel(ICorpusContext context) {
            m_context = context;
        }

        public Guid Guid { get; }           = System.Guid.NewGuid();
        public string Status { get; set; }  = "init";
        public float Progress { get; set; } = 0.0f;
        public int Priority { get; set; }   = 0;
        public IEnumerable<KeyValueProperty> Properties { get; set; } = DefaultProperties;

        private static IEnumerable<KeyValueProperty> DefaultProperties {
            get {
                var properties = new List<KeyValueProperty>();
                //properties.Add(
                //        new KeyValueProperty(
                //            "timeout",  /* key */
                //            "30",  /* defaultValue */
                //            "seconds"  /* type */
                //            ));
                //properties.Add(
                //        new KeyValueProperty(
                //            "website",  /* key */
                //            "http://example.com",  /* defaultValue */
                //            "url"  /* type */
                //            ));
                return new List<KeyValueProperty>();
            }
        }
        public void Run() {
            /* Implement a fake learner that simply waits for a while and
             * periodically increments the progress */
            while (Progress < 1.0f) {
                Thread.Sleep(5000);
                Progress += 0.1f;
            }
        }

        public static string DisplayName {
            get { return "GLoVe Learning Model"; }
        }
        public static string Description {
            get {
                return
                    @"An implementation of the GLoVe word vector representation model
                      as described here: http://nlp.stanford.edu/projects/glove/";
            }
        }
        public static string ContentType {
            get {
                return "vast knowledge";
            }
        }
    }
}

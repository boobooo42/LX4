using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using LexicalAnalyzer.Services;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace LexicalAnalyzer.LearningModels
{
    public class GloveLearningModel : ILearningModel
    {
        private ICorpusContext m_context;
        private CooccurArgs cooccurArgs;
        private GloveArgs gloveArgs;
        private ShuffleArgs shuffleArgs;
        private VocabCountArgs vocabCountArgs;

        public GloveLearningModel(ICorpusContext context) {
            m_context = context;
            cooccurArgs = new CooccurArgs();
            gloveArgs = new GloveArgs();
            shuffleArgs = new ShuffleArgs();
            vocabCountArgs = new VocabCountArgs();
        }

        public Guid Guid { get; }           = System.Guid.NewGuid();
        public string Status { get; set; }  = "init";
        public float Progress { get; set; } = 0.0f;
        public int Priority { get; set; }   = 0;
        public IEnumerable<KeyValueProperty> Properties { get; set; } = DefaultProperties;
        public string Type
        {
            get
            {
                return this.GetType().FullName;
            }
        }

        private static IEnumerable<KeyValueProperty> DefaultProperties {
            get {
                var properties = new List<KeyValueProperty>();
                // TODO: as many of these as we like:
                //properties.Add(
                //        new KeyValueProperty(
                //            "[selection from arg structs]",  /* key */
                //            "[default val of selection]",  /* defaultValue */
                //            "[units or type description]"  /* type */
                //            ));
                return properties;
            }
        }
        public void Run() {
            test(2);
            /* Waits and does nothing */
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

        [DllImport("deeplearning.dll")]
        public static extern int test(int foo);
    }

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
        public ShuffleArgs() {
            verbose = 0;
            memory = 4.0f;
            arraySize = -1;
            tempFile = "temp_shuffle";
            mode = 0;
        }
    }
    public class VocabCountArgs {
        int verbose, maxVocab, minCount, mode;
        public VocabCountArgs() {
            verbose = 0;
            maxVocab = -1;
            minCount = 1;
            mode = 0;
        }
    }
}

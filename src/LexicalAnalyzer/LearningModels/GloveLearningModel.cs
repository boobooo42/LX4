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
        private IMerkleTreeContext m_context;
        private CooccurArgs cooccurArgs;
        private GloveArgs gloveArgs;
        private ShuffleArgs shuffleArgs;
        private VocabCountArgs vocabCountArgs;

        public GloveLearningModel(IMerkleTreeContext context) {
            m_context = context;
            cooccurArgs = new CooccurArgs();
            FillCooccurArgs(ref cooccurArgs);
            gloveArgs = new GloveArgs();
            FillGloveArgs(ref gloveArgs);
            shuffleArgs = new ShuffleArgs();
            FillShuffleArgs(ref shuffleArgs);
            vocabCountArgs = new VocabCountArgs();
            FillVocabCountArgs(ref vocabCountArgs);
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

        public static IEnumerable<KeyValueProperty> DefaultProperties {
            get {
                var properties = new List<KeyValueProperty>();
                // TODO: as many of these as we like:
                //properties.Add(
                //        new KeyValueProperty(
                //            "[selection from arg structs]",  /* key */
                //            "[default val of selection]",  /* defaultValue */
                //            "[units or type description]"  /* type */
                //            ));
                properties.Add(
                    new KeyValueProperty(
                        "cSymmetric", "1", "boolean"
                        ));
                properties.Add(
                    new KeyValueProperty(
                        "cWindowSize", "15", "integer"
                        ));
                properties.Add(
                    new KeyValueProperty(
                        "cMaxProduct", "-1", "integer"
                        ));
                properties.Add(
                    new KeyValueProperty(
                        "gVectorSize", "50", "integer"
                        ));
                properties.Add(
                    new KeyValueProperty(
                        "gIter", "15", "integer"
                        ));
                properties.Add(
                    new KeyValueProperty(
                        "gEta", "0.05", "float"
                        ));
                properties.Add(
                    new KeyValueProperty(
                        "gAlpha", "0.75", "float"
                        ));
                properties.Add(
                    new KeyValueProperty(
                        "gXMax", "100.0", "float"
                        ));
                properties.Add(
                    new KeyValueProperty(
                        "vMaxVocab", "-1", "integer"
                        ));
                properties.Add(
                    new KeyValueProperty(
                        "vMinVocab", "1", "integer"
                        ));
                return properties;
            }
        }
        public void Run() {
            string corpus = "corpus.txt";
            string vocab = "vocab.txt";
            string cooccurence = "cooccurence.bin";
            string cooccurenceShuf = "cooccurence.shuf.bin";
            string gradsq = "gradsq";
            string save = "vectors";

            vocabCount(ref vocabCountArgs, corpus, vocab);
            Progress = 0.25f;
            cooccur(ref cooccurArgs, corpus, vocab, cooccurence);
            Progress = 0.5f;
            shuffle(ref shuffleArgs, cooccurence, cooccurenceShuf);
            Progress = 0.75f;
            glove(ref gloveArgs, cooccurenceShuf, vocab, save, gradsq);
            Progress = 1.0f;
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

        [DllImport("learning64.dll")]
        static extern int test(int foo);


        public IResult Result {
            get {
                /* TODO: Output Glove results */
                TestResult result = new TestResult();
                return result;
            }
        }

        public void FillCooccurArgs(ref CooccurArgs args) {
            args.verbose = 1;
            args.symmetric = 1;
            args.windowSize = 15;
            args.memory = 4;
            args.maxProduct = -1;
            args.overflowLength = -1;
            args.overflowFile = "overflow";
            args.mode = 0;
        }
        [DllImport("learning64.dll")]
        static extern int cooccur(ref CooccurArgs args, string corpusInFile, string vocabInFile, string cooccurOutFile);

        public void FillGloveArgs(ref GloveArgs args) {
            args.verbose = 1;
            args.vectorSize = 50;
            args.threads = 8;
            args.iter = 15;
            args.eta = 0.05f;
            args.alpha = 0.75f;
            args.xMax = 100.0f;
            args.binary = 0;
            args.model = 2;
            args.saveGradsq = 0;
            args.checkpointEvery = 0;
            args.mode = 0;
        }
        [DllImport("learning64.dll")]
        static extern int glove(ref GloveArgs args, string shufCooccurInFile, string vocabInFile, string gloveOutFile, string gradsqOutFile);

        public void FillShuffleArgs(ref ShuffleArgs args) {
            args.verbose = 1;
            args.memory = 4.0f;
            args.arraySize = -1;
            args.tempFile = "temp_shuffle";
            args.mode = 0;
        }
        [DllImport("learning64.dll")]
        static extern int shuffle(ref ShuffleArgs args, string cooccurInFile, string shufCooccurOutFile);

        public void FillVocabCountArgs(ref VocabCountArgs args) {
            args.verbose = 1;
            args.maxVocab = -1;
            args.minCount = 1;
            args.mode = 0;
        }
        [DllImport("learning64.dll")]
        static extern int vocabCount(ref VocabCountArgs args, string corpusInFile, string vocabOutFile);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CooccurArgs {
        public int verbose, symmetric, windowSize;
        public float memory;
        public int maxProduct, overflowLength;
        public string overflowFile;
        public int mode;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct GloveArgs {
        public int verbose, vectorSize, threads, iter;
        public float eta, alpha, xMax;
        public int binary, model;
        public int saveGradsq, checkpointEvery, mode;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct ShuffleArgs {
        public int verbose;
        public float memory;
        public int arraySize;
        public string tempFile;
        public int mode;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct VocabCountArgs {
        public int verbose, maxVocab, minCount, mode;
    }
}

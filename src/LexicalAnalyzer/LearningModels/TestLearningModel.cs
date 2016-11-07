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
    public class TestLearningModel : ILearningModel
    {
        private ICorpusContext m_context;

        public TestLearningModel(ICorpusContext context) {
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
                properties.Add(
                        new KeyValueProperty(
                            "timeout",  /* key */
                            "30",  /* defaultValue */
                            "seconds"  /* type */
                            ));
                properties.Add(
                        new KeyValueProperty(
                            "website",  /* key */
                            "http://example.com",  /* defaultValue */
                            "url"  /* type */
                            ));
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
            get { return "Test Learning Model"; }
        }
        public static string Description {
            get {
                return
                    @"A learning model used for testing purposes. This learning model does
                    not actually learn any things; it simply pretends to
                    learn things.";
            }
        }
        public static string ContentType {
            get {
                return "vast knowledge";
            }
        }
    }
}

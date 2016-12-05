using LexicalAnalyzer.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LexicalAnalyzer.Models {
    public class ZipfResult : IResult {
        public class DataObject {
            public IEnumerable<RankFrequencyPair> Words {
                get; set;
            }
            public IEnumerable<RankFrequencyPair> Characters {
                get; set;
            }
        }

        /* Private members */
        DataObject m_data;

        /* Constructors */
        public ZipfResult(
                IEnumerable<RankFrequencyPair> words,
                IEnumerable<RankFrequencyPair> characters)
        {
            m_data = new DataObject();
            m_data.Words = words;
            m_data.Characters = characters;
        }

        /* Properties */
        public string Type {
            get { return this.GetType().FullName; }
        }

        public string Data {
            get {
                return JsonConvert.SerializeObject(m_data);
            }
        }
    }
}

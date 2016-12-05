using System.Collections.Generic;
using System;

namespace LexicalAnalyzer.Models {
    public class LearningModel /* : ILearningModel */ {
        /* Private members */
        List<KeyValueProperty> m_properties;

        /* Constructors */
        public LearningModel() {
            m_properties = new List<KeyValueProperty>();
        }

        /* Properties */
        public string Guid { get; set; }
        public string Status { get; set; }
        public float Progress { get; set; }
        public int Priority { get; set; }

        public IEnumerable<KeyValueProperty> Properties {
            get {
                return m_properties;
            }
            set {
                m_properties = new List<KeyValueProperty>(value);
            }
        }
    }
}

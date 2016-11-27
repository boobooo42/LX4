using System.Collections.Generic;

namespace LexicalAnalyzer.Models {
    public class LearningModelType {
        /* Private members */
        private List<KeyValueProperty> m_properties;

        /* Attributes */
        public string Type { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }

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

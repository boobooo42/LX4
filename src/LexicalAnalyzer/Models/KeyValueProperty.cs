using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexicalAnalyzer.Models
{
    public class KeyValueProperty
    {
        /* Private members */
        private string m_key, m_value, m_type;

        public KeyValueProperty(string key, string defaultValue = "", string type = "text") {
            m_key = key;
            m_value = defaultValue;
            m_type = type;
        }

        /* Public accessors */
        public string Key {
            get {
                return m_key;
            }
        }

        public string Type {
            get {
                return m_type;
            }
        }

        public string Value {
            get {
                return m_value;
            }
            set {
                m_value = value;
            }
        }
    }
}

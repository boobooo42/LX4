using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.LearningModels;
using LexicalAnalyzer.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System;

namespace LexicalAnalyzer.Services {
    public class LearningModelFactory : ILearningModelFactory {
        /* Private members */
        private List<Type> m_learningModelTypes;
        private ICorpusContext m_context;

        /* Constructors */
        public LearningModelFactory(ICorpusContext context) {
            m_context = context;
            m_learningModelTypes = new List<Type>();

            /* Fill our array of learning model types */
            // m_learningModelTypes.Add(typeof(TestLearningModel));
            m_learningModelTypes.Add(typeof(TestLearningModel));
            m_learningModelTypes.Add(typeof(ZipfLearningModel));
            m_learningModelTypes.Add(typeof(GloveLearningModel));

            /* TODO: Add learning models from DLL assemblies */

            /* Ensure that each learning model type implements
             * ILearningModel */
            foreach (Type t in m_learningModelTypes) {
                Debug.Assert(t.GetInterfaces()
                        .Contains(typeof(ILearningModel)));
            }

            /* FIXME: Ensure that each learning model type implements the
             * needed static methods (with appropriate signatures) */
        }

        public IEnumerable<LearningModelType> LearningModelTypes {
            get {
                List<LearningModelType> result = new List<LearningModelType>();

                foreach (Type t in m_learningModelTypes) {
                    LearningModelType lmt = new LearningModelType();
                    /* Store the fully qualified name of the implementing
                     * class */
                    lmt.Type = t.FullName;
                    /* Invoke the respective static methods for this type */
                    lmt.DisplayName = (string)t
                        .GetProperty("DisplayName",
                                BindingFlags.Public | BindingFlags.Static)
                        .GetValue(null, null);
                    lmt.Description = (string)t
                        .GetProperty("Description",
                                BindingFlags.Public | BindingFlags.Static)
                        .GetValue(null);
                    lmt.Properties = (IEnumerable<KeyValueProperty>)
                        t.GetProperty("DefaultProperties",
                                BindingFlags.Public | BindingFlags.Static)
                        .GetValue(null);
                    result.Add(lmt);
                }

                return result;
            }
        }

        public ILearningModel BuildLearningModel(string type) {
            Type t = m_learningModelTypes.Find(
                    elem => { return elem.FullName == type; });
            if (t == null)
                return null;
            if (!t.GetInterfaces().Contains(typeof(ILearningModel)))
                return null;
            object[] arguments = new object[] { m_context };
            return (ILearningModel)Activator.CreateInstance(t, arguments);
        }
    }
}

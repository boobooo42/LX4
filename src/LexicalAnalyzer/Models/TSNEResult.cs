using LexicalAnalyzer.Interfaces;
using Newtonsoft.Json;

namespace LexicalAnalyzer {
    public class TSNEResult : IResult {
        public string Type {
            get {
                return this.GetType().FullName;
            }
        }

        public string Data {
            get {
                return "TODO";
            }
        }
    }
}

using LexicalAnalyzer.Interfaces;
using Newtonsoft.Json;

namespace LexicalAnalyzer.Models {
    public class ZipfResult : IResult {
        /* Private members */

        public string Type {
            get { return this.GetType().FullName; }
        }

        public string Data {
            get {
                return JsonConvert.SerializeObject("TODO");
            }
        }
    }
}

using LexicalAnalyzer.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LexicalAnalyzer.Models {
    public class TestResult : IResult {
        public string Type {
            get { return this.GetType().FullName; }
        }

        public string Data {
            get {
                var data = new List<string>();
                data.Add("foo");
                data.Add("bar");
                return JsonConvert.SerializeObject(data);
            }
        }
    }
}

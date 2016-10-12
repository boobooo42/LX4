using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexicalAnalyzer.Database
{
    public class CorpusBlob
    {
        public string CorpusHash { get; set; }
        public List<string> FileBlobList { get; set; }
    }
}

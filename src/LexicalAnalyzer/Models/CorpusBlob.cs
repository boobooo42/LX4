using System.Collections.Generic;

namespace LexicalAnalyzer.Models {
    /// <summary>
    /// The POCO object for collecting content into a corpus.
    /// </summary>
    public class CorpusBlob : MerkleNode {
        public IEnumerable<ContentBlob> Children { get; set; }
    }
}

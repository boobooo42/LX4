using System.Collections.Generic;

namespace LexicalAnalyzer.Models {
    /// <summary>
    /// The POCO object for collecting content into a corpus.
    /// </summary>
    public class CorpusBlob : MerkleNode {
        /* Constructors */
        public CorpusBlob() {
            this.Content = new List<ContentBlob>();
        }

        public List<ContentBlob> Content { get; set; }
    }
}

using LexicalAnalyzer.Interfaces;

namespace LexicalAnalyzer.Models {
    /// <summary>
    /// The POCO object for content stored in the corpus.
    /// </summary>
    public class ContentBlob : MerkleNode {
        public byte[] Content { get; set; }
        public string Hash { get; set; }
    }
}

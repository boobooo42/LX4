using LexicalAnalyzer.Interfaces;

namespace LexicalAnalyzer.Models {
    /// <summary>
    /// The abstract POCO object inherited by any Merkle node in the database.
    /// </summary>
    public class MerkleNode {
        public MerkleHash Hash { get; set; }

        public IEnumerable<MerkleNode> Children { get; set; }

        public bool Validate() {
            /* TODO: Traverse the Merkle node hierarchy to compute the SHA256
             * hash */
            return false;
        }
    }
}

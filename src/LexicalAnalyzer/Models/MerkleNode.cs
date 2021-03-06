using LexicalAnalyzer.Interfaces;
using System.Collections.Generic;

namespace LexicalAnalyzer.Models {
    /// <summary>
    /// The abstract POCO object inherited by any Merkle node in the database.
    /// </summary>
    public class MerkleNode : IMerkleNode {

        /* Accessors */
        public string Hash { get; set; }
        public string Type { get; set; }

        /* Implement the Merkle node flyweight behavior */
        public bool IsFlyweight { get; set; }

        /* Used to determine which nodes are subject to garbage collection */
        public bool Pinned { get; set; }

        public IEnumerable<MerkleNode> Children { get; set; }

        public bool Validate() {
            /* TODO: Traverse the Merkle node hierarchy to compute the SHA256
             * hash */
            return false;
        }
    }
}

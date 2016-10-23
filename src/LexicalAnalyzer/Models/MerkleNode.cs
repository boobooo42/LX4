using LexicalAnalyzer.Interfaces;
using System.Collections.Generic;

namespace LexicalAnalyzer.Models {
    /// <summary>
    /// The abstract POCO object inherited by any Merkle node in the database.
    /// </summary>
    public class MerkleNode : IMerkleNode {
        /* Private members */
        private MerkleHash m_hash;

        public MerkleHash Hash {
            get {
                if (m_hash == null) {
                    /* TODO: Compute the hash for this node? This should be
                     * immutable though... */
                }
                return m_hash;
            }
        }

        /* TODO: Implement the Merkle node flyweight behavior */
        public bool IsFlyweight { get { return false; } }

        public IEnumerable<MerkleNode> Children { get; set; }

        public bool Validate() {
            /* TODO: Traverse the Merkle node hierarchy to compute the SHA256
             * hash */
            return false;
        }
    }
}

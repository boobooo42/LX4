 using System.Diagnostics;
using System.Linq;

namespace LexicalAnalyzer.Models {
    /// <summary>
    /// SHA256 hash type used to uniquely identify any Merkle node and its
    /// children
    /// </summary>
    public class MerkleHash {
        /* Attributes */
        public byte[] Value { get; set; }

        /* Constants */
        private const int SHA256_NUM_BYTES = 256 / 8;
        private const int HASH_NUM_BYTES = SHA256_NUM_BYTES;

        /* Public methods */
        public override bool Equals(System.Object obj) {
            if (obj == null)
                return false;

            MerkleHash other = obj as MerkleHash;
            if ((System.Object)other == null)
                return false;

            Debug.Assert(this.Value.Length == HASH_NUM_BYTES);
            Debug.Assert(other.Value.Length == HASH_NUM_BYTES);
            return this.Value.SequenceEqual(other.Value);
        }

        public bool Equals(MerkleHash other) {
            if (other == null)
                return false;

            Debug.Assert(this.Value.Length == HASH_NUM_BYTES);
            Debug.Assert(other.Value.Length == HASH_NUM_BYTES);
            return this.Value.SequenceEqual(other.Value);
        }
    }
}

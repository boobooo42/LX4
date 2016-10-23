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
        private const int SHA256_NUM_HEX_DIGITS = 256 / 4;
        private const int HASH_NUM_BYTES = SHA256_NUM_BYTES;

        /* Constructors */
        public MerkleHash(string hash) {
            if (hash.Length != SHA256_NUM_HEX_DIGITS) {
                return;
            }
            Value = new byte[SHA256_NUM_BYTES];
            /* Convert the hash from base 16 to binary */
            /* NOTE: We store our hash as a big-endian byte array */
            for (int i = 0; i < hash.Length; ++i) {
                byte hexValue = CharToHex(hash[i]);
                if (hexValue == 255) {
                    /* We found a character that is not a valid hexadecimal
                     * character */
                    Value = null;
                    return;
                }
                int valueIndex = i / 2;
                if (i % 2 == 0) {
                    /* Reading the high-order nibble */
                    Value[valueIndex] = (byte)(hexValue << 4);
                } else {
                    /* Reading the low-order nibble */
                    Value[valueIndex] |= hexValue;
                }
            }
        }

        /* Private methods */
        private static byte CharToHex(char c) {
            switch (c) {
                case '0':
                    return 0;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
                case 'a':
                case 'A':
                    return 10;
                case 'b':
                case 'B':
                    return 11;
                case 'c':
                case 'C':
                    return 12;
                case 'd':
                case 'D':
                    return 13;
                case 'e':
                case 'E':
                    return 14;
                case 'f':
                case 'F':
                    return 15;
            }
            return 255;
        }

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

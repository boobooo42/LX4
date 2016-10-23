using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LexicalAnalyzer.Models {
    /// <summary>
    /// SHA256 hash type used to uniquely identify any Merkle node and its
    /// children
    /// </summary>
    public class MerkleHash {
        /* Constants */
        private const int SHA256_NUM_BYTES = 256 / 8;
        private const int SHA256_NUM_HEX_DIGITS = 256 / 4;
        private const int HASH_NUM_BYTES = SHA256_NUM_BYTES;

        /* Private members */
        private byte[] m_value;

        /* Attributes */
        public byte[] Value {
            get { return m_value; }
            set {
                if (value == null) {
                    m_value = null;
                    return;
                }
                Debug.Assert(value.Length == SHA256_NUM_BYTES);
                m_value = value;
            }
        }
        public string Hash {
            get {
                if (this.Value == null)
                    return "";

                StringBuilder sb = new StringBuilder(SHA256_NUM_HEX_DIGITS);
                /* NOTE: The value is stored in big-endian format, so we simply
                 * print hex digits starting from the beginning of the value
                 * byte array */
                for (int i = 0; i < this.Value.Length; ++i) {
                    byte b = this.Value[i];

                    /* Append the hex digit for the high-order nibble */
                    sb.Append(ValueToHex((byte)(b >> 4)));

                    /* Append the hex digit for the low-order nibble */
                    sb.Append(ValueToHex((byte)(b & ((1 << 4) - 1))));
                }
                return sb.ToString();
            }

            set {
                string hash = value;
                /* TODO: Convert the Value to a string */
                if (hash.Length != SHA256_NUM_HEX_DIGITS) {
                    return;
                }
                Value = new byte[SHA256_NUM_BYTES];
                /* Convert the hash from base 16 to binary */
                /* NOTE: We store our hash as a big-endian byte array */
                for (int i = 0; i < hash.Length; ++i) {
                    byte hexValue = HexToValue(hash[i]);
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
        }

        /* Constructors */
        public MerkleHash(string hash) {
            this.Hash = hash;
        }

        /* Private methods */
        private static byte HexToValue(char c) {
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

        private static char ValueToHex(byte b) {
            switch (b) {
                case 0:
                    return '0';
                case 1:
                    return '1';
                case 2:
                    return '2';
                case 3:
                    return '3';
                case 4:
                    return '4';
                case 5:
                    return '5';
                case 6:
                    return '6';
                case 7:
                    return '7';
                case 8:
                    return '8';
                case 9:
                    return '9';
                case 10:
                    return 'a';
                case 11:
                    return 'b';
                case 12:
                    return 'c';
                case 13:
                    return 'd';
                case 14:
                    return 'e';
                case 15:
                    return 'f';
            }
            Debug.Assert(false);
            return 'x';
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

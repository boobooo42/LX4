using LexicalAnalyzer.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace LexicalAnalyzer.Models {
    /// <summary>
    /// The POCO object for learning model results stored in the corpus.
    /// </summary>
    public class ResultBlob : MerkleNode {
        /* Constructors */
        public ResultBlob() {
            this.Type = "ResultBlob";
        }
        public ResultBlob(IResult result) {
            this.Type = "ResultBlob";
            this.ResultType = result.Type;
            this.Data = result.Data;
        }

        public string ResultType { get; set; }
        public string Data { get; set; }

        public string ComputeHash() {
            var sha256 = SHA256.Create();
            var sbHash = new StringBuilder();
            /* Compute a hash of the result type */
            byte[] typeHash = sha256.ComputeHash(
                    Encoding.UTF8.GetBytes(this.ResultType));
            foreach (byte b in typeHash)
            {
                sbHash.Append(string.Format("{0:x2}", b));
            }
            /* Compute a hash of the result data */
            byte[] dataHash = sha256.ComputeHash(
                    Encoding.UTF8.GetBytes(this.Data));
            foreach (byte b in dataHash)
            {
                sbHash.Append(string.Format("{0:x2}", b));
            }
            /* Compute the hash of the concatenated hashes */
            var sbFinalHash = new StringBuilder();
            byte[] finalHash = sha256.ComputeHash(
                    Encoding.UTF8.GetBytes(sbHash.ToString()));
            foreach (byte b in finalHash)
            {
                sbFinalHash.Append(string.Format("{0:x2}", b));
            }
            string finalHashString = sbFinalHash.ToString();
            return finalHashString;
        }
    }
}

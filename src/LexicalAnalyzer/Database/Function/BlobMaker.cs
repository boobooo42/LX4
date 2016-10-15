using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LexicalAnalyzer.Database.Function
{
    public class BlobMaker
    {
        public void computeHash ()
        {
            string stringContent = "";
            string hashResult = "";
            SHA256 mySha = SHA256.Create();
            IFileRepository FileRepository = new FileRepository();
            List<File> files = FileRepository.GetAll();
            foreach (File f in files)
            {
                hashResult = "";
                stringContent = f.FileContents;
                //// Converts the string to bytes
                byte[] byteData = Encoding.UTF8.GetBytes(stringContent);
                //// hash data computed by SHA256
                byte[] hashData = mySha.ComputeHash(byteData);
                //// puts the bytes into a single readable string with the format
                foreach (byte v in hashData)
                {
                    hashResult = hashResult + String.Format("{0:x2}", v);
                }
                //// sets the FileHash to the computed sha256 hash

                f.FileHash = hashResult;

            }
        }

    }
}

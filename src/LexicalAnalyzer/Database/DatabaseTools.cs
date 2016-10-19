using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LexicalAnalyzer.Database
{
    public static class DatabaseTools
    {
        public static void CheckDatabase()
        {
            IInfoRepository info = new InfoRepository();
            int version = info.GetVersion();
            // Check an info table
            // If the table doesn't exist, call ProvisionDatabase().
        }

        public static void ProvisionDatabase()
        {
            // Create tables.
        }
        public static void createFile(string contents)
        {
            //// Creates a  file  with contents to be passed into InsertIntoDatabase method
            File fi = new File();
            fi.FileContents = contents;
            InsertIntoDatabase(fi);

        }

        public static void InsertIntoDatabase(File file)
        {
            // Create FileRepository object and insert file.
            IFileRepository FileRepository = new FileRepository();
            String hash=computeHash(file);
            file.FileHash = hash;
            FileRepository.insertFile(file);
            
        }
        /// return the computed hash of a given file
        public static string computeHash(File fi)
        {
            string stringContent = "";
            string hashResult = "";
            SHA256 mySha = SHA256.Create();
            stringContent = fi.FileContents;
            hashResult = "";
            //// Converts the string to bytes
            byte[] byteData = Encoding.UTF8.GetBytes(stringContent);
            //// hash data computed by SHA256
            byte[] hashData = mySha.ComputeHash(byteData);
            //// puts the bytes into a single readable string with the format
            foreach (byte v in hashData)
            {
                hashResult = hashResult + String.Format("{0:x2}", v);
            }
            return hashResult;
        }
        ///returns the computed corpus hash
        public  static string computeCorpusHash()
        {
            string fileHash = "";
            string stringHash = "";
            string hashResult = "";
            SHA256 mySha = SHA256.Create();
            IFileRepository FileRepository = new FileRepository();
            List<File> files = FileRepository.GetAll();
            foreach(File f in files)
            {
                fileHash = f.FileHash;
                stringHash = stringHash + fileHash;
                //// gets byte data of the string
                byte[] byteData = Encoding.UTF8.GetBytes(stringHash);
                //// compute the hash for the corpus
                byte[] hashData = mySha.ComputeHash(byteData);
                /// makes the byte data to readable formate
                foreach (byte v in hashData)
                {
                    hashResult = hashResult + String.Format("{0:x2}", v);
                }
            }

            return hashResult;
        }

    }
}

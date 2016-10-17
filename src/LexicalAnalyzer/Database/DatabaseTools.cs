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
            InfoRepository info = new InfoRepository();
            int version = info.GetVersion();
            // Check an info table
            // If the table doesn't exist, call ProvisionDatabase().
        }

        public static void ProvisionDatabase()
        {
            // Create tables.
        }

        public static void InsertIntoDatabase(File file)
        {
            // Create FileRepository object and insert file.

            String hash=computeHash(file);
            file.FileHash = hash;
        }
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

    }
}

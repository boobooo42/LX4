using System;
using System.Collections.Generic;
using System.Linq;
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
        }


    }
}

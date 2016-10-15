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
            // Check an info table
            // If the table doesn't exist, call CreateDatabase()
        }

        public static void CreateDatabase()
        {
            // Create a database
            // Then call ProvisionDatabase() to set up the tables.
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

using Dapper;
using LexicalAnalyzer.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;

namespace LexicalAnalyzer.DataAccess {
    public static class DatabaseTools {
        /* Constants */
        public const int DatabaseVersion = 1;

        /* Public methods */
        public static void InitializeDatabase(string connectionString)
        {
            try {
                /* TODO: Support more than one database provider */
                using (var cn = new SqlConnection(connectionString)) {
                    cn.Open();
                    /* Try to retrieve the database version from the currently
                     * existing database */
                    IEnumerable<DatabaseInfo> result =
                        cn.Query<DatabaseInfo>(@"
                            SELECT *
                            FROM la.Info");
                    Debug.Assert(result.Count() == 1);
                    int version = result.First().Version;
                    /* Note that we only have one version of the database at
                     * the moment */
                    /* TODO: Add database migration support here */
                    Debug.Assert(version == DatabaseVersion);
                }
            } catch {
                /* The database does not appear to have our Info table; we
                 * assume that it is empty and that we must provision tables */
                using (var cn = new SqlConnection(connectionString)) {
                    cn.Open();
                    /* Create the schema */
                    cn.Execute(@"CREATE SCHEMA la");
                    /* Iterate over each resource used for the schema */
                    foreach (string resourceString in new string[] {
                            "LexicalAnalyzer.schema.sql",
                            "LexicalAnalyzer.constraints.sql"})
                    {
                        string sql = ReadAssemblyResource(resourceString);
                        Debug.Assert(sql != null);
                        /* Provision the database by executing the SQL code in the
                         * schema */
                        /* TODO: Support more than one database provider */
                        RunSqlBatch(sql, cn);
                    }
                    /* Insert a record of the database version */
                    cn.Execute(@"
                            INSERT INTO la.Info
                            ( Version )
                            VALUES ( @version )",
                            new { version = DatabaseVersion });
                }
            }
        }

        public static void AddExampleData(string connectionString) {
            /* Read the example data from the assembly resourc */
            string sql = ReadAssemblyResource(
                    "LexicalAnalyzer.example_data.sql");
            Debug.Assert(sql != null);
            try {
                /* TODO: Support more than one database provider */
                using (var cn = new SqlConnection(connectionString)) {
                    cn.Open();
                    RunSqlBatch(sql, cn);
                }
            } catch (SqlException e) {
                /* Since the transaction failed, we can assume that we already
                 * have example data in our database */
                Debug.WriteLine("Failed to add example data; assuming we already have data");
            }
        }

        private static string ReadAssemblyResource(string name) {
            /* Read string from our assembly resources
             * Thanks to:
             * <http://codeopinion.com/asp-net-core-embedded-resource/>
             */
            var assembly = Assembly.GetEntryAssembly();
            var resourceStream = assembly.GetManifestResourceStream(name);
            string result;
            using (var reader = new StreamReader(
                        resourceStream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        private static void RunSqlBatch(string sql, IDbConnection cn) {
            /* NOTE: This is a very naive SQL statement parser. It cannot
             * handle things like semicolons in quotes or comments. A real
             * parser of some sort would be needed to do this properly, but so
             * long as our schema does not have any weird semicolons we will be
             * fine. */
            Regex re = new Regex(";");
            string[] statements = re.Split(sql);
            using (IDbTransaction tran = cn.BeginTransaction()) {
                try {
                    /* Execute the all SQL statements in series */
                    foreach (string statement in statements) {
                        cn.Execute(statement, transaction: tran);
                    }
                    tran.Commit();
                } catch (SqlException e) {
                    tran.Rollback();
                    for (int i = 0; i < e.Errors.Count; ++i) {
                        Debug.WriteLine("SQL Error: " + e.Errors[i].ToString());
                    }
                    throw;
                }
            }
        }
    }
}

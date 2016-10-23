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
            Debug.WriteLine("Connection String: " + connectionString);
            try {
                /* TODO: Support more than one database provider */
                using (var cn = new SqlConnection(connectionString)) {
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
                /* Read schema.sql from our assembly resources
                 * Thanks to:
                 * <http://codeopinion.com/asp-net-core-embedded-resource/>
                 */
                var assembly = Assembly.GetEntryAssembly();
                var resourceStream = assembly.GetManifestResourceStream(
                        "LexicalAnalyzer.schema.sql");
                string sql;
                using (var reader = new StreamReader(
                            resourceStream, Encoding.UTF8))
                {
                    sql = reader.ReadToEnd();
                }
                Debug.Assert(sql != null);
                /* Provision the database by executing the SQL code in the
                 * schema */
                /* TODO: Support more than one database provider */
                using (var cn = new SqlConnection(connectionString)) {
                    cn.Open();
                    RunSqlBatch(sql, cn);
                }
            }
        }

        public static void RunSqlBatch(string sql, IDbConnection cn) {
            /* NOTE: This is a very naive SQL statement parser. It cannot
             * handle things like semicolons in quotes or comments. A real
             * parser of some sort would be needed to do this properly, but so
             * long as our schema does not have any weird semicolons we will be
             * fine. */
            Regex re = new Regex(";");
            string[] statements = re.Split(sql);
            using (IDbTransaction tran = cn.BeginTransaction()) {
                try {
                    foreach (string statement in statements) {
                        cn.Execute(statement, transaction: tran);
                    }
                    tran.Commit();
                } catch {
                    tran.Rollback();
                    throw;
                }
            }
        }
    }
}

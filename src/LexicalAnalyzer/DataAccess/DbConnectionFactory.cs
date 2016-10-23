using LexicalAnalyzer.Interfaces;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using System.Data;

namespace LexicalAnalyzer.DataAccess {
    public class DbConnectionFactory : IDbConnectionFactory {
        /* Private members */
        IOptions<DatabaseOptions> m_options;

        /* Constructors */
        DbConnectionFactory(IOptions<DatabaseOptions> options) {
            m_options = options;
        }

        /* Public methods */
        public IDbConnection CreateConnection() {
            /* TODO: Support different database providers */
            var connection = new SqlConnection(
                    m_options.Value.ConnectionString);
            connection.Open();
            return connection;
        }
    }
}

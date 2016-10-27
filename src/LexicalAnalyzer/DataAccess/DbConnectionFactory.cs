using LexicalAnalyzer.Interfaces;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using System.Data;

namespace LexicalAnalyzer.DataAccess {
    /* Provide the IDbConnection object through dependency injection.  Note
     * that the answers at this URL suggest making a db connection factory or
     * provider:
     * <http://stackoverflow.com/questions/14523166/is-there-a-simple-way-to-use-dependency-injection-on-my-connections>
     */
    public class DbConnectionFactory : IDbConnectionFactory {
        /* Private members */
        IOptions<DatabaseOptions> m_options;

        /* Constructors */
        public DbConnectionFactory(IOptions<DatabaseOptions> options) {
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

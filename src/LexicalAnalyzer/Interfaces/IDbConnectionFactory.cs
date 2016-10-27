using System.Data;

namespace LexicalAnalyzer.Interfaces {
    /// <summary>
    /// Interface that facilitates passing database connection factories
    /// through dependency injection.
    /// </summary>
    public interface IDbConnectionFactory {
        IDbConnection CreateConnection();
    }
}

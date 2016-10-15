using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using LexicalAnalyzer;

namespace LexicalAnalyzer
{
    public class FileRepository : IFileRepository
    {
        private IDbConnection db = new SqlConnection("Data Source=LAPTOP-B7NR0KID;Initial Catalog=master;Integrated Security=SSPI;User Id=Max;MultipleActiveResultSets=True");
        
        public List<File> GetAll()
        {
            return this.db.Query<File>("SELECT * FROM [dbo].[File]").ToList();
        }
    }
}

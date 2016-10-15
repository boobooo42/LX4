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
        private IDbConnection db = new SqlConnection(@"Data Source=ICEY\MSSQLSERVERAW;Initial Catalog=master;Integrated Security=True");
        
        public List<File> GetAll()
        {
            return this.db.Query<File>("SELECT * FROM [dbo].[File]").ToList();
        }

        public void setHash(File file,string result)
        {
            string query = "UPDATE [dbo].[File] SET FileHash = @hash WHERE FileID = @ID;";    
            this.db.Execute(query, new { hash = result, ID = file.FileID });
           
        }
    }
}

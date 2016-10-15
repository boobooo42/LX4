using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using System;
namespace LexicalAnalyzer
{
    public class FileRepository : IFileRepository
    {
        private IDbConnection db = new SqlConnection(@"Data Source=ICEY\MSSQLSERVERAW;Initial Catalog=master;Integrated Security=True");

        public List<File> GetAll()
        {
             return db.Query<File>("SELECT * FROM [dbo].[File]").ToList();
        }

        public void setHash(byte[] result)
        {
            string rule = "UPDATE EMPLOYEE SET NAME = @Name WHERE Id = @Id";

            db.Execute( )
        }
    }
}

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
        /// sets all the hashs in fileTable  (Took blob maker out this may not be used)
        public void setHash(File file,string result)
        {
            string query = "UPDATE [dbo].[File] SET FileHash = @hash WHERE FileID = @ID;";    
            this.db.Execute(query, new { hash = result, ID = file.FileID });
           
        }
        /// check the table to see if the hash is already contained if not insert into database
        public void insertFile(File file)
        {
            /// expensive way to get idCount (need optimize)
            IFileRepository FileRepository = new FileRepository();
            int idCount;
            List<File>files= FileRepository.GetAll();
            idCount=files.Count()+1;
            string query = "IF NOT EXISTS (SELECT * FROM [dbo].[File] WHERE FileHash = @hash) INSERT INTO [dbo].[File](FileID, FileHash, FileContents) VALUES(@ID, @hash, @contents);";
            this.db.Execute(query, new {ID = idCount, hash = file.FileHash, contents = file.FileContents, });
        }
    }
}

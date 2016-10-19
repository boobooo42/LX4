using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace LexicalAnalyzer.Database
{
    public class InfoRepository : IInfoRepository
    {
        private IDbConnection db = new SqlConnection(@"Data Source=ICEY\MSSQLSERVERAW;Initial Catalog=master;Integrated Security=True");
        public int GetVersion()
        {
            return 1;
        }
    }
}

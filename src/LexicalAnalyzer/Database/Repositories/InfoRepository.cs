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
        /* FIXME: This database handle should be created in a central location */
        private IDbConnection db = new SqlConnection(@"Data Source=ICEY\MSSQLSERVERAW;Initial Catalog=master;Integrated Security=True");
        public int GetVersion()
        {
            return 1;
        }
    }
}

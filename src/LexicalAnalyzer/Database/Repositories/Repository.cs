using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexicalAnalyzer.Database.Repositories
{
    public class Repository<T> : IRepository<T> where T : IGuid
    {
    }
}

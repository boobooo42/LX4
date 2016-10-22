using LexicalAnalyzer.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

/// <remarks>
/// <p>
/// This interface is inspired by <a
/// href="http://deviq.com/repository-pattern/">this excellent article from
/// DevIQ</a>.
/// </p>
/// </remarks>
namespace LexicalAnalyzer.Database.Repositories
{
    public interface IRepository<T> where T : IGuid
    {
        T GetByHash( hash);
        IEnumerable<T> List();
        IEnumerable<T> List(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Delete(T entity);
        /* FIXME: What is edit needed for? This is all by reference isn't it?
         * Probably because we need to generate UPDATE queries whenever
         * anything changes. */
        void Edit(T entity);
    }
}

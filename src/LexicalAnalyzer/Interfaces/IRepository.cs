using System.Collections.Generic;

namespace LexicalAnalyzer.Interfaces {
    public interface IRepository<T>
    {
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
        T GetById(long id);
        IEnumerable<T> List();
//        IEnumerable<T> List(Expression<Func<T, bool>> predicate);
    }
}

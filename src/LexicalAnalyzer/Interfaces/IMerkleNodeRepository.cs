using LexicalAnalyzer.Models;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using System;

/// <summary>
/// Generic interface for repositories that provide access to Merkle nodes.
/// </summary>
/// <remarks>
/// <p>
/// This interface is inspired by
/// <a href="http://deviq.com/repository-pattern/">
/// this excellent article from DevIQ
/// </a>.
/// </p>
/// </remarks>
namespace LexicalAnalyzer.Interfaces
{
    public interface IMerkleNodeRepository<T> where T : IMerkleNode
    {
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
        T GetByHash(MerkleHash hash);
        IEnumerable<T> List();
//        IEnumerable<T> List(Expression<Func<T, bool>> predicate);
    }

    public interface IMerkleNodeRepository
        : IMerkleNodeRepository<MerkleNode>
    {
    }
}

using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System;

namespace LexicalAnalyzer.DataAccess {
    /// <summary>
    /// Implements a generic MerkleNodeRepository with a database backend.
    /// </summary>
    public abstract class MerkleNodeRepository<T>
        : IMerkleNodeRepository<T>
        where T : IMerkleNode
    {
        /* Private members */
        IDbConnectionFactory m_connectionFactory;

        /* Constructors */
        public MerkleNodeRepository(IDbConnectionFactory connectionFactory) {
            m_connectionFactory = connectionFactory;
        }

        /* Methods */
        protected abstract string TableName { get; }

        protected IDbConnection Connection() {
            return m_connectionFactory.CreateConnection();
        }

        public abstract void Add(T entity);
        public abstract void Delete(T entity);
        /* NOTE: Most Merkle nodes should not have an update method. A few
         * exceptions are the Merkle nodes whose hashes are known before their
         * values are computed, such as the neural nets and the query
         * results. */
        public abstract void Update(T entity);
        public abstract T GetByHash(MerkleHash hash);
        public abstract IEnumerable<T> List();
//        public abstract IEnumerable<T> List(Expression<Func<T, bool>> predicate);
    }
}

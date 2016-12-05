using Dapper;
using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Linq;
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
            var conn =  m_connectionFactory.CreateConnection();
            return conn;
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

    public class MerkleNodeRepository
        : MerkleNodeRepository<MerkleNode>,
          IMerkleNodeRepository
    {
        public MerkleNodeRepository(IDbConnectionFactory connectionFactory)
            : base(connectionFactory)
        {
        }

        protected override string TableName {
            get { return "MerkleNode"; }
        }

        public override void Add(MerkleNode node) {
            Debug.Assert(!node.IsFlyweight);
            using (IDbConnection cn = this.Connection()) {
                cn.Execute(@"
                        INSERT INTO la.MerkleNode
                            (Hash, Type)
                            VALUES (@Hash, @Type)",
                        node);
            }
        }

        public override void Delete(MerkleNode node) {
            using (IDbConnection cn = this.Connection()) {
                cn.Execute(@"
                        DELETE FROM la.MerkleNode
                            WHERE Hash=@Hash",
                        node);
                /* TODO: Delete associated Merkle edges */
                /* TODO: Schedule garbage collection of orphaned nodes */
            }
        }

        public override void Update(MerkleNode node) {
            /* NOTE: The Merkle nodes themselves are immutable */
        }

        public override MerkleNode GetByHash(MerkleHash hash) {
            MerkleNode node = null;
            using (IDbConnection cn = this.Connection()) {
                IEnumerable<MerkleNode> result = cn.Query<MerkleNode>(@"
                        SELECT *
                        FROM la.MerkleNode
                        WHERE MerkleNode.Hash=@Hash
                        ", new { Hash = hash });
                if (result.Any()) {
                    node = result.First();
                    node.IsFlyweight = true;  /* A generic merkle node will
                                                 certainly be missing all of
                                                 its content */
                    /* TODO: Recursively get the children of this Merkle
                     * node */
                }
            }
            return node;
        }

        public override IEnumerable<MerkleNode> List() {
            IEnumerable<MerkleNode> list = null;
            using (IDbConnection cn = this.Connection()) {
                list = cn.Query<MerkleNode>(@"
                        SELECT Hash
                        FROM la.MerkleNode
                        ");
                foreach (MerkleNode node in list) {
                    node.IsFlyweight = true;  /* A generic merkle node will
                                                 certainly be missing all of
                                                 its content */
                    /* TODO: Recursively get the children of this Merkle
                     * node */
                }
            }
            return list;
        }
    }
}

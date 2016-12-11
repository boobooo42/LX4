using Dapper;
using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace LexicalAnalyzer.DataAccess {
    public class ResultBlobRepository
        : MerkleNodeRepository<ResultBlob>,
        IResultBlobRepository
    {
        public ResultBlobRepository(IDbConnectionFactory connectionFactory)
            : base(connectionFactory)
        {
        }

        protected override string TableName {
            get { return "ResultBlob"; }
        }

        public override void Add(ResultBlob result) {
            Debug.Assert(!result.IsFlyweight);
            result.Hash = result.ComputeHash();
            using (IDbConnection cn = this.Connection()) {
                using (IDbTransaction trans = cn.BeginTransaction()) {
                    try {
                        /* Check for existing results with the same hash */
                        var queryResult = cn.Query<ResultBlob>(@"
                            SELECT Hash
                            FROM la.ResultBlob
                            WHERE Hash = @Hash
                            ", result,
                            transaction: trans);
                        if (queryResult.Any()) {
                            Debug.Assert(queryResult.Count() == 1);
                            /* The result is already in the database; nothing
                             * to do */
                            trans.Rollback();
                            return;
                        }
                        /* Create a Merkle node for the result */
                        Debug.Assert(result.Type.Equals("ResultBlob"));
                        cn.Execute(@"
                            INSERT INTO la.MerkleNode
                            ( Hash, Type, Pinned )
                            VALUES ( @Hash, @Type, @Pinned )
                            ", result,
                            transaction: trans);
                        /* Store this result in the database */
                        cn.Execute(@"
                            INSERT INTO la.ResultBlob
                            ( Hash, ResultType, Data )
                            VALUES ( @Hash, @ResultType, @Data )
                            ", result,
                            transaction: trans);
                        /* Create edges for the nodes that this result depends
                         * on */
                        foreach (var child in result.Children) {
                            cn.Execute(@"
                                INSERT INTO la.MerkleEdge
                                ( ParentHash, ChildHash )
                                VALUES ( @ParentHash, @ChildHash )
                                ", new {
                                    ParentHash = result.Hash,
                                    ChildHash = child.Hash
                                },
                                transaction: trans);
                        }
                        trans.Commit();
                    } catch (SqlException e) {
                        trans.Rollback();
                        for (int i = 0; i < e.Errors.Count; ++i) {
                            Debug.WriteLine(string.Format(
                                        "SQL Error: {0}",
                                        e.Errors[i].ToString()));
                        }
                        throw;
                    }
                }
            }
        }

        public override void Delete(ResultBlob result) {
            /* TODO */
            Debug.Assert(false);
        }

        public override void Update(ResultBlob result) {
            /* NOTE: Results are immutable; this method should never be
             * called. */
            Debug.Assert(false);
        }

        public override ResultBlob GetByHash(string hash) {
            /* TODO */
            Debug.Assert(false);
            return null;
        }

        public override IEnumerable<ResultBlob> List() {
            /* TODO */
            Debug.Assert(false);
            return null;
        }
    }
}

using Dapper;
using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace LexicalAnalyzer.DataAccess {
    public class ContentBlobRepository
        : MerkleNodeRepository<ContentBlob>
    {
        public ContentBlobRepository(IDbConnectionFactory connectionFactory)
            : base(connectionFactory)
        {
        }

        protected override string TableName {
            get { return "ContentBlob"; }
        }

        public override void Add(ContentBlob content) {
            Debug.Assert(!content.IsFlyweight);
            using (IDbConnection cn = this.Connection()) {
                cn.Execute(
                        "INSERT INTO ContentBlob " +
                            "(Hash, Contents) " +
                            "VALUES (@Hash, @Contents)",
                        content);
            }
        }

        public override void Delete(ContentBlob content) {
            using (IDbConnection cn = this.Connection()) {
                cn.Execute(
                        "DELETE FROM ContentBlob " +
                            "WHERE Hash=@Hash",
                        content);
            }
        }

        public override void Update(ContentBlob content) {
            using (IDbConnection cn = this.Connection()) {
                cn.Execute(
                        "DELETE FROM ContentBlob " +
                            "WHERE Hash=@Hash",
                        content);
            }
        }

        public override ContentBlob GetByHash(MerkleHash hash) {
            ContentBlob content = default(ContentBlob);
            using (IDbConnection cn = this.Connection()) {
                IEnumerable<ContentBlob> result = cn.Query<ContentBlob>(@"
                        SELECT *
                        FROM ContentBlob
                        LEFT OUTER JOIN MerkleNode
                            ON (MerkleNode.Hash = ContentBlob.Hash)
                        WHERE ContentBlob.Hash=@Hash
                        ", new { Hash = hash });
                if (result.Any()) {
                    content = result.First();
                }
            }
            return content;
        }

        public override IEnumerable<ContentBlob> List() {
            IEnumerable<ContentBlob> list = null;
            using (IDbConnection cn = this.Connection()) {
                list = cn.Query<ContentBlob>(@"
                        SELECT Hash
                        FROM ContentBlob
                        LEFT OUTER JOIN MerkleNode
                            ON (MerkleNode.Hash = ContentBlob.Hash)
                        ");
            }
            return list;
        }
    }
}

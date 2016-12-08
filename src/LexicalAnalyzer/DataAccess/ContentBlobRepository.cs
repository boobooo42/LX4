using Dapper;
using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace LexicalAnalyzer.DataAccess {
    public class ContentBlobRepository
        : MerkleNodeRepository<ContentBlob>,
          IContentBlobRepository
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
                        "INSERT INTO la.ContentBlob (Hash, Contents) VALUES (@Hash, @Contents)",
                        new { Hash = content.Hash, Contents = content.Contents });
                /* TODO: Insert the corresponding MerkleNode record */
            }
        }

        public override void Delete(ContentBlob content) {
            using (IDbConnection cn = this.Connection()) {
                cn.Execute(
                        "DELETE FROM la.ContentBlob WHERE Hash=@Hash",
                        new { Hash = content.Hash });
            }
            /* TODO: Delete the corresponding MerkleNode record */
        }

        public override void Update(ContentBlob content) {
            using (IDbConnection cn = this.Connection()) {
                /* TODO */
                /*
                cn.Execute(
                        "DELETE FROM ContentBlob " +
                            "WHERE Hash=@Hash",
                        new {Hash = content.Hash});
                        */
            }
        }

        public override ContentBlob GetByHash(string hash) {
            ContentBlob content = null;
            using (IDbConnection cn = this.Connection()) {
                IEnumerable<ContentBlob> result = cn.Query<ContentBlob>(@"
                    SELECT Hash, Contents
                    FROM la.ContentBlob
                    WHERE la.ContentBlob.Hash=@Hash
                    ", new { Hash = hash });
                if (result.Any()) {
                    Debug.Assert(result.Count() == 1);
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
                        FROM la.ContentBlob
                        LEFT OUTER JOIN MerkleNode
                            ON (MerkleNode.Hash = ContentBlob.Hash)
                        ");
                /* FIXME: This is almost certainly broken. We need to use the
                 * Dapper multi mapping facilities to make the list of child
                 * nodes. */
            }
            return list;
        }
    }
}

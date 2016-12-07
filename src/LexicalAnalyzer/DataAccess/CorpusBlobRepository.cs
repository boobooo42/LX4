using Dapper;
using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LexicalAnalyzer.DataAccess {
    public class CorpusBlobRepository
        : MerkleNodeRepository<CorpusBlob>,
        ICorpusBlobRepository
    {
        /* Constructors */
        public CorpusBlobRepository(IDbConnectionFactory connectionFactory)
            : base(connectionFactory)
        {
        }

        /* Public methods */
        protected override string TableName {
            get { return "CorpusBlob"; }
        }

        public override void Add(CorpusBlob corpusBlob) {
            /* TODO: Not sure if this method is even needed */
            Debug.Assert(false);
        }

        public override void Delete(CorpusBlob corpusBlob) {
            /* TODO: Delete the corpus blob with the given hash, so long as
             * there are no foreign keys referncing it */
        }

        public override void Update(CorpusBlob corpusBlob) {
            /* NOTE: The corpus blob is immutable; it should never be
             * updated */
            Debug.Assert(false);
        }

        public override CorpusBlob GetByHash(string hash) {
            CorpusBlob corpusBlob = null;
            using (IDbConnection cn = this.Connection()) {
                IEnumerable<CorpusBlob> result = cn.Query<CorpusBlob>(
                    @" SELECT * FROM la.CorpusBlob
                        LEFT OUTER JOIN MerkleNode
                            ON (MerkleNode.Hash = CorpusBlob.Hash)
                        WHERE CorpusBlob.Hash=@Hash
                        ", new { Hash = hash });
                /* FIXME: This is almost certainly broken. We need to use the
                 * Dapper multi mapping facilities to make the list of child
                 * nodes. */
                if (result.Any()) {
                    corpusBlob = result.First();
                }
            }
            return corpusBlob;
        }

        public override IEnumerable<CorpusBlob> List() {
            IEnumerable<CorpusBlob> list = null;
            using (IDbConnection cn = this.Connection()) {
                list = cn.Query<CorpusBlob>(@"
                        SELECT Hash
                        FROM la.CorpusBlob
                        LEFT OUTER JOIN MerkleNode
                            ON (MerkleNode.Hash = CorpusBlob.Hash)
                        ");
                /* FIXME: This is almost certainly broken. We need to use the
                 * Dapper multi mapping facilities to make the list of child
                 * nodes. */
            }
            return list;
        }

        public CorpusBlob GetByCorpusID(long id) {
            CorpusBlob corpusBlob = null;
            using (var conn = this.Connection()) {
                /* TODO: Start a database transaction */
                using (IDbTransaction trans = conn.BeginTransaction()) {
                    try {
                        /* Look in the Corpus table for a corpus with the given
                         * ID. */
                        IEnumerable<Corpus> result = conn.Query<Corpus>(@"
                                SELECT Id, Name, Description, Locked, Hash
                                FROM la.Corpus
                                WHERE Id=@Id
                                ", new { Id = id },
                                transaction: trans);
                        if (!result.Any()) {
                            Debug.WriteLine(string.Format(
                                "Error: Could not find corpus with ID '{1}'",
                                id));
                            return null;
                        }
                        Debug.Assert(result.Count() == 1);
                        Corpus corpus = result.First();
                        /* Check the hash field of this corpus. If it is null,
                         * then the blob likely does not yet exist. */
                        if (true) {
                            /* Create a corpus blob from the existing state of this
                             * corpus */
                            corpus.Hash = this.GetCorpusBlob(
                                    id, conn, trans);
                            /* Update the hash field of the corpus record */
                            conn.Execute(@"
                                UPDATE la.Corpus
                                SET Hash = @Hash
                                WHERE ID = @Id
                                ", new { Id = id, Hash = corpus.Hash },
                                transaction: trans);
                        }
                        /* Retrieve the CorpusBlob object. */
                        IEnumerable<CorpusBlob> blobResult =
                            conn.Query<CorpusBlob,ContentBlob,CorpusBlob>(@"
                                SELECT
                                    corpus.Hash,
                                    content.Hash
                                FROM la.CorpusBlob AS corpus
                                LEFT JOIN la.MerkleNode AS corpus_mn
                                    ON corpus.Hash = corpus_mn.Hash
                                LEFT JOIN la.MerkleEdge AS edge
                                    ON edge.ParentHash = corpus_mn.Hash
                                LEFT JOIN la.MerkleNode AS content_mn
                                    ON content_mn.Hash = edge.ChildHash
                                LEFT JOIN la.ContentBlob AS content
                                    ON content.Hash = content_mn.Hash
                                WHERE corpus.Hash = @Hash
                                ",
                                map: (corp,cont) => {
                                    /* Note that we only set the hash of the
                                     * content blob; we do not actually fill
                                     * its contents. */
                                    corp.Content.Add(cont);
                                    return corp;
                                },
                                param: new { Hash = corpus.Hash },
                                splitOn: "Hash,Hash",
                                transaction: trans);
                        Debug.Assert(blobResult.Any());
                        //Debug.Assert(blobResult.Count() == 1);
                        corpusBlob = blobResult.First();
                        foreach(CorpusBlob blob in blobResult)
                        {
                            corpusBlob.Content.Add(blob.Content[0]);
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
            return corpusBlob;
        }

        /* Private methods */
        private string GetCorpusBlob(
                long corpusId,
                IDbConnection conn,
                IDbTransaction trans)
        {
            /* Get all of the content hashes currently in this corpus */
            IEnumerable<CorpusContent> contentResult = conn.Query<CorpusContent>(@"
                    SELECT Id, Hash, Name, Type,
                        
                        ScraperType,
                        DownloadDate,
                        DownloadURL
                    FROM la.CorpusContent
                    WHERE CorpusId=@CorpusId
                    ", new { CorpusId = corpusId },
                    transaction: trans);

            string hash = ComputeContentHash(contentResult);

            /* Check for an existing corpus blob with this hash */
            IEnumerable<CorpusBlob> blobResult = conn.Query<CorpusBlob>(@"
                    SELECT Hash
                    FROM la.CorpusBlob
                    WHERE Hash=@Hash
                    ", new { Hash = hash },
                    transaction: trans);
            if (!blobResult.Any()) {
                /* The corpus blob does not yet exist; we must create
                 * it */
                this.GenerateCorpusBlob(
                        contentResult,
                        hash,
                        conn,
                        trans);
            } else {
                Debug.Assert(blobResult.Count() == 1);
                /* There is an existing corpus blob with an identical
                 * hash, so we do not need to create a new corpus blob. */
                /* TODO: Verify that the content in the existing content blob
                 * matches our content. */
            }

            return hash;
        }

        private static string ComputeContentHash(
                IEnumerable<CorpusContent> contentResult)
        {
            /* Ensure hashes are lowercase */
            /* NOTE: This is a side-effect of the ComputeContentHash()
             * function. Eh, oh well. */
            foreach (var content in contentResult) {
                content.Hash = content.Hash.ToLower();
            }
            /* Sort the hashes alphabetically */
            var sorted = contentResult.OrderBy(content => content.Hash);
            /* Concatenate the hashes and compute the hash of the resulting
             * string */
            StringBuilder sb = new StringBuilder();
            Debug.WriteLine("sorted content hashes:");  /* XXX */
            foreach (var content in sorted) {
                Debug.WriteLine("  hash: {1}", content.Hash.ToLower());
                sb.Append(content.Hash.ToLower());
            }
            var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(
                    Encoding.UTF8.GetBytes(sb.ToString()));
            StringBuilder sbHash = new StringBuilder();
            foreach (byte b in hash) {
                sbHash.Append(string.Format("{0:x2}", b));
            }
            string hashString = sbHash.ToString();
            Debug.WriteLine("CorpusBlob hash: {1}", hashString);

            return hashString;
        }

        private void GenerateCorpusBlob(
                IEnumerable<CorpusContent> content,
                string hash,
                IDbConnection conn,
                IDbTransaction trans)
        {
            /* Create a Merkle node for the corpus blob */
            conn.Execute(@"
                    INSERT INTO la.MerkleNode ( Hash, Type, Pinned )
                    VALUES ( @Hash, @Type, @Pinned )
                    ", new {
                        Hash = hash,
                        Type = "CorpusBlob",
                        Pinned = false
                    },
                    transaction: trans);
            conn.Execute(@"
                    INSERT INTO la.CorpusBlob ( Hash )
                    VALUES ( @Hash )
                    ", new { Hash = hash },
                    transaction: trans);
            /* Create a Merkle edge for each of the content blobs in this
             * corpus blob */
            foreach (var blob in content) {
                conn.Execute(@"
                    INSERT INTO la.MerkleEdge( ParentHash, ChildHash )
                    VALUES ( @ParentHash, @ChildHash )
                    ", new {
                        ParentHash = hash,
                        ChildHash = blob.Hash
                    },
                    transaction: trans);
            }
        }
    }
}

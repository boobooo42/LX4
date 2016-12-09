using Dapper;
using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LexicalAnalyzer.DataAccess
{
    public class CorpusContentRepository : ICorpusContentRepository
    {
        /* Private members */
        private IDbConnectionFactory m_connectionFactory;
        /* Constructors */
        public CorpusContentRepository(
                IDbConnectionFactory connectionFactory)
        {
            m_connectionFactory = connectionFactory;
        }

        /* Private methods */
        private IDbConnection Connection()
        {
            var conn = m_connectionFactory.CreateConnection();
            return conn;
        }

        /* Public methods */
        public void Add(CorpusContent content)
        {
            Debug.Assert(content.Id == -1);
            using (var conn = this.Connection())
            {
                using (IDbTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        //string contentGuid = content.ScraperGuid.ToString();
                        //string contentText = System.Text.Encoding.UTF8.GetString(content.Content);
                        //conn.Execute(@"IF NOT EXISTS
                        //(SELECT 1 FROM la.Corpus
                        //    WHERE Id = @Id)BEGIN                            
                        //    INSERT INTO la.Corpus
                        //    (Id, Name, Description)
                        //    VALUES (@Id, @Name, @Description)
                        //    END ", new
                        //{
                        //    Id = 1,
                        //    Name = "stuff",
                        //    Text = "more stuff",
                        //}, transaction: tran);
                        conn.Execute(@"IF NOT EXISTS
                        (SELECT 1 FROM la.CorpusContent
                            WHERE Hash = @Hash)BEGIN 
                        INSERT INTO la.CorpusContent
                            (CorpusId, Hash, Name, Type, ScraperGuid, ScraperType, DownloadDate,
                             DownloadURL, Long, Lat )
                            VALUES ( @CorpusId, @Hash, @Name, @Type, @ScraperGuid, @ScraperType, @DownloadDate,
                                @DownloadURL, @Long, @Lat) END
                            ", new
                        {
                            CorpusId = content.CorpusId,
                            Hash = content.Hash,
                            Name = content.Name,
                            Type = content.Type,
                            ScraperGuid = content.ScraperGuid,
                            ScraperType = content.ScraperType,
                            DownloadDate = content.DownloadDate,
                            DownloadUrl = content.URL,
                            Long = content.Long,
                            Lat = content.Lat
                        }, transaction: tran);
                        conn.Execute(@"IF NOT EXISTS
                        (SELECT 1 FROM la.MerkleNode
                            WHERE Hash = @Hash)BEGIN                            
                            INSERT INTO la.MerkleNode
                            (Hash, Type, Pinned)
                            VALUES (@Hash, @Type, @Pinned)
                            END ", new
                        {
                            Hash = content.Hash,
                            Type = content.Type,
                            Pinned = 1,
                        }, transaction: tran
                                     );
                        conn.Execute(@"IF NOT EXISTS
                        (SELECT 1 FROM la.ContentBlob
                            WHERE Hash = @Hash)BEGIN
                            INSERT INTO la.ContentBlob
                            (Hash, Contents)
                             VALUES (@Hash, @Contents)
                            END",
                             new
                             {
                                 Hash = content.Hash,
                                 Contents = System.Text.Encoding.UTF8.GetString(content.Content),
                             }, transaction: tran);
                        tran.Commit();
                    }
                    catch (SqlException e)
                    {
                        tran.Rollback();
                        for (int i = 0; i < e.Errors.Count; ++i)
                        {
                            Debug.WriteLine("SQL Error: " + e.Errors[i].ToString());
                        }
                        throw;

                    }

                    /* TODO: Check for flyweight CorpusContent objects */
                    /* TODO: Make sure the contents are somehow added to the Merkle
                     * tree as a ContentBlob */
                    /* TODO: If we also add a ContentBlob here, it would be nice to
                     * do everything as a single transaction */
                }
            }

        }

        public void Delete(CorpusContent content)
        {
            Debug.Assert(content.Id != -1);
            using (var conn = this.Connection())
            {
                conn.Execute(@"
                        DELETE FROM la.CorpusContent
                            WHERE Id=@Id
                            ", new { Id = content.Id });

                /* TODO: Schedule garbage collection in the Merkle tree in case
                 * ContentBlob associated with this corpus content has been
                 * orphaned */
            }
        }

        public void Update(CorpusContent content)
        {
            Debug.Assert(content.Id != -1);
            /* TODO */
        }

        public CorpusContent GetById(long id)
        {
            CorpusContent content = null;
            using (var conn = this.Connection())
            {
                /* TODO: Support two different calls, one for fetching
                 * flyweight objects and one for heavyweight objects */
                IEnumerable<CorpusContent> result =
                    conn.Query<CorpusContent>(@"
                        SELECT Id, Hash, Name, Type,
                            ScraperGuid,
                            ScraperType,
                            DownloadDate,
                            DownloadURL
                        FROM la.CorpusContent
                        WHERE Id=@Id
                            ", new { Id = id });
                if (result.Any())
                {
                    Debug.Assert(result.Count() == 1);
                    content = result.First();
                }
            }
            return content;
        }

        public IEnumerable<CorpusContent> List(int? corpusId)
        {
            IEnumerable<CorpusContent> list = null;
            using (var conn = this.Connection())
            {
                using (IDbTransaction tran = conn.BeginTransaction())
                {
                    /* NOTE: It would be a bad idea to fetch a heavyweight list of
                     * all of the corpus content in the database, so we fetch
                     * flyweight objects here */
                    list = conn.Query<CorpusContent>(@"
                    SELECT Id, Hash, Name, Type,
                        ScraperGuid,
                        ScraperType,
                        DownloadDate,
                        DownloadURL
                    FROM la.CorpusContent
                    WHERE CorpusId=@CorpusId
                        ", new { CorpusId = corpusId }, transaction: tran);
                }

            }
            return list;
        }
    }
}
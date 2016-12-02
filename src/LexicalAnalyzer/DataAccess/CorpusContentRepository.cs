using Dapper;
using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

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
                        //   conn.Execute(@"");
                        conn.Execute(@" 
                        INSERT INTO la.CorpusContent
                            (CorpusId, Hash, Name, Type, ScraperGuid, ScraperType,
                             DownloadURL, Long, Lat )
                            VALUES ( @CorpusId, @Hash, @Name, @Type, @ScraperGuid, @ScraperType,
                                @DownloadURL, @Long, @Lat )
                            ", new
                        {
                            CorpusId = 1,
                            Hash = content.Hash,
                            Name = content.Name,
                            Type = content.Type,
                            ScraperGuid = content.ScraperGuid.ToString(),
                            ScraperType = content.ScraperType,
                            DownloadUrl = content.URL,
                            Long = content.Long,
                            Lat = content.Lat
                        }, transaction: tran);
                        //conn.Execute(@"INSERT INTO la.ContentBlob
                        //    (Hash, Contents)
                        //     VALUES (@Hash, @Contents)",
                        //     new
                        //     {
                        //         Hash = content.Hash,
                        //         Contents = content.Content,
                        //     }, transaction: tran);
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
                        ", new { CorpusId = corpusId });

            }
            return list;
        }
    }
}
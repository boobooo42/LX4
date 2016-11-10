using Dapper;
using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using System.Collections.Generic;
using System.Data;
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
                    /// adds to the CorpusContentTable
                conn.Execute(@" IF NOT EXISTS (SELECT * FROM la.CorpusContent WHERE Hash =@Hash)
                        INSERT INTO la.CorpusContent
                            (CorpusId, Hash, Name, Type,
                             DownloadURL, Long, Lat )
                            VALUES ( @CorpusId, @Hash, @Name, @Type,
                                @DownloadURL, @Long, @Lat )
                            ", new
                {

                    CorpusId = 1,
                    Hash = content.Hash,
                    Name = content.Name,
                    Type = content.Type,
                    DownloadUrl = content.URL,
                    Long = content.Long,
                    Lat = content.Lat
                });
                /// adds to the MerkleNodeTable
                conn.Execute(@" IF NOT EXISTS (SELECT * FROM la.MerkleNode WHERE Hash =@Hash)
                        INSERT INTO la.MerkleNode (Hash,Type,Pinned) VALUES (@Hash,@Type,@Pinned)",
                    new { Hash = content.Hash, Type = "ContentBlob", Pinned = 0 });
                /// adds to the ContentBlobTable
                conn.Execute(@" IF NOT EXISTS (SELECT * FROM la.ContentBlob WHERE Hash =@Hash)
                            INSERT INTO la.ContentBlob (Hash, Contents) VALUES (@Hash, @Contents)",
                        new { Hash = content.Hash, Contents = content.Content });



                //// makes the contentBlob to add to contentBlobTable
                //ContentBlob conBlob = new ContentBlob();
                // ContentBlobRepository conRepo = new ContentBlobRepository(m_connectionFactory);
                //conBlob.Hash = content.Hash;
                // conBlob.Content = content.Content;
                //conRepo.Add(conBlob);

                /* TODO: Check for flyweight CorpusContent objects */
                /* TODO: Make sure the contents are somehow added to the Merkle
                 * tree as a ContentBlob */
                /* TODO: If we also add a ContentBlob here, it would be nice to
                 * do everything as a single transaction */
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
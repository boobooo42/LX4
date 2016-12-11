using Dapper;
using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace LexicalAnalyzer.DataAccess {
    public class CorpusRepository : ICorpusRepository {
        /* Private members */
        private IDbConnectionFactory m_connectionFactory;

        /* Constructors */
        public CorpusRepository(IDbConnectionFactory connectionFactory)
        {
            m_connectionFactory = connectionFactory;
        }
        public List<ContentBlob> GetAll()
        {
            using (IDbConnection cn = this.Connection())
            {
                return cn.Query<ContentBlob>("SELECT Hash FROM la.ContentBlob").ToList();
            }

        }

        /* Private methods */
        private IDbConnection Connection() {
            var conn = m_connectionFactory.CreateConnection();
            return conn;
        }

        /* Public methods */
        public void Add(Corpus corpus)
        {
            Debug.Assert(corpus.Id == -1);
            using (var conn = this.Connection())
            {
                conn.Execute(@"
                    INSERT INTO la.Corpus ( Name, Description, Locked )
                    VALUES ( @Name, @Description, @Locked )
                    ", new {
                        Name = corpus.Name,
                        Description = corpus.Description,
                        Locked = corpus.Locked });
            }

        }

        public void Delete(Corpus corpus) {
            Debug.Assert(corpus.Id != -1);
            using (var conn = this.Connection()) {
              
                        /* Delete the corpus and all of its content in one
                         * database transaction */
                        conn.Execute(
                            @" DELETE FROM la.Corpus WHERE Id=@Id ",
                            new { Id = corpus.Id });
            }
        }

        public void Update(Corpus corpus) {
            using (var conn = this.Connection())
            {
                conn.Execute(@"
                    UPDATE la.Corpus
                    SET Name = @Name,
                        Description = @Description,
                        Locked = @Locked 
                    WHERE Id = @Id
                    ", new {
                        Name = corpus.Name,
                        Description = corpus.Description,
                        Locked = corpus.Locked,
                        Id = corpus.Id });
            }
        }

        public Corpus GetById(long id) {
            Corpus corpus = null;
            using (var conn = this.Connection()) {
                IEnumerable<Corpus> result = conn.Query<Corpus>(@"
                        SELECT Id, Name, Description, Locked
                        FROM la.Corpus
                        WHERE Id=@Id
                        ", new { Id = id });
                if (result.Any()) {
                    Debug.Assert(result.Count() == 1);
                    corpus = result.First();
                }
            }
            return corpus;
        }

        public IEnumerable<Corpus> List(int? id) {
            IEnumerable<Corpus> list = null;
            using (var conn = this.Connection()) {
                list = conn.Query<Corpus>(@"
                        SELECT Id, Name, Description, Locked
                        FROM la.Corpus
                        ");
            }
            return list;
        }
    }
}

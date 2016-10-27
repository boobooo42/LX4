using LexicalAnalyzer.Interfaces;

namespace LexicalAnalyzer.DataAccess {
    public class CorpusContext : ICorpusContext {
        /* Private members */
        IDbConnectionFactory m_connectionFactory;
        CorpusRepository m_corpusRepository;
        CorpusContentRepository m_corpusContentRepository;

        /* Constructors */
        public CorpusContext(IDbConnectionFactory connectionFactory) {
            m_connectionFactory = connectionFactory;
        }

        /* Accessors */
        public ICorpusRepository CorpusRepository {
            get {
                if (m_corpusRepository == null) {
                    m_corpusRepository =
                        new CorpusRepository(m_connectionFactory);
                }
                return m_corpusRepository;
            }
        }

        public ICorpusContentRepository CorpusContentRepository {
            get {
                if (m_corpusContentRepository == null) {
                    m_corpusContentRepository =
                        new CorpusContentRepository(m_connectionFactory);
                }
                return m_corpusContentRepository;
            }
        }
    }
}

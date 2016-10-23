using LexicalAnalyzer.Interfaces;
using System.Data;

namespace LexicalAnalyzer.DataAccess {
    public class MerkleTreeContext : IMerkleTreeContext {
        /* Private members */
        IDbConnectionFactory m_connectionFactory;
        ContentBlobRepository m_contentBlobRepository;

        /* TODO: Provide the IDbConnection object through dependency injection.
         * Note that the answers at this URL suggest making a db connection
         * factory or provider:
         * <http://stackoverflow.com/questions/14523166/is-there-a-simple-way-to-use-dependency-injection-on-my-connections>
         */
        public MerkleTreeContext(IDbConnectionFactory connectionFactory) {
            m_connectionFactory = connectionFactory;
        }

        /* Accessors */
        public IContentBlobRepository ContentBlobRepository {
            get {
                if (m_contentBlobRepository == null) {
                    m_contentBlobRepository =
                        new ContentBlobRepository(m_connectionFactory);
                }
                return (IContentBlobRepository)m_contentBlobRepository;
            }
        }
        /*
        ICorpusBlobRepository CorpusBlobRepository {
            get {
                if (m_corpusBlobRepository == null) {
                    m_corpusBlobRepository =
                        new CorpusBlobRepository(m_connectionFactory);
                }
                return m_corpusBlobRepository;
            }
        }
        */
    }
}

using LexicalAnalyzer.Interfaces;
using System.Data;

namespace LexicalAnalyzer.DataAccess {
    public class MerkleTreeContext : IMerkleTreeContext {
        /* Private members */
        IDbConnectionFactory m_connectionFactory;
        MerkleNodeRepository m_merkleNodeRepository;
        ContentBlobRepository m_contentBlobRepository;
        CorpusBlobRepository m_corpusBlobRepository;
        ResultBlobRepository m_resultBlobRepository;

        public MerkleTreeContext(IDbConnectionFactory connectionFactory) {
            m_connectionFactory = connectionFactory;
        }

        /* Accessors */
        public IMerkleNodeRepository MerkleNodeRepository {
            get {
                if (m_merkleNodeRepository == null) {
                    m_merkleNodeRepository =
                        new MerkleNodeRepository(m_connectionFactory);
                }
                return m_merkleNodeRepository;
            }
        }

        public IContentBlobRepository ContentBlobRepository {
            get {
                if (m_contentBlobRepository == null) {
                    m_contentBlobRepository =
                        new ContentBlobRepository(m_connectionFactory);
                }
                return m_contentBlobRepository;
            }
        }

        public ICorpusBlobRepository CorpusBlobRepository {
            get {
                if (m_corpusBlobRepository == null) {
                    m_corpusBlobRepository =
                        new CorpusBlobRepository(m_connectionFactory);
                }
                return m_corpusBlobRepository;
            }
        }

        public IResultBlobRepository ResultBlobRepository {
            get {
                if (m_resultBlobRepository == null) {
                    m_resultBlobRepository =
                        new ResultBlobRepository(m_connectionFactory);
                }
                return m_resultBlobRepository;
            }
        }
    }
}

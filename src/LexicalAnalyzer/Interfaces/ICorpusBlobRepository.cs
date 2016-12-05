using LexicalAnalyzer.Models;

namespace LexicalAnalyzer.Interfaces {
    public interface ICorpusBlobRepository
        : IMerkleNodeRepository<CorpusBlob>
    {
        CorpusBlob GetByCorpusID(long id);
    }
}

using LexicalAnalyzer.Models;
using System.Collections.Generic;

namespace LexicalAnalyzer.Interfaces {
    public interface ICorpusBlobRepository
        : IMerkleNodeRepository<CorpusBlob>
    {
        CorpusBlob GetByCorpusID(long id);
    }
}

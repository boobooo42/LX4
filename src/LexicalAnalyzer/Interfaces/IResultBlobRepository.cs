using LexicalAnalyzer.Models;
using System.Collections.Generic;

namespace LexicalAnalyzer.Interfaces {
    public interface IResultBlobRepository
        : IMerkleNodeRepository<ResultBlob>
    {
    }
}

using LexicalAnalyzer.Models;

namespace LexicalAnalyzer.Interfaces {
    /* FIXME: I'm not sure if we really need this interface. --Jonathan */
    public interface IMerkleNode {
        MerkleHash Hash { get; set; }
    }
}

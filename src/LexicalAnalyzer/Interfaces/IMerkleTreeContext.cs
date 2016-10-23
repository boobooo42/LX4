namespace LexicalAnalyzer.Interfaces {
    /// <summary>
    /// Interface for classes that implement a data access context for the
    /// Merkle tree.
    /// </summary>
    /// <remarks>
    /// <p>
    /// Classes implementing this interface are essentially factories for
    /// objects that implement the various IMerkleNodeRepository interfaces.
    /// </p>
    /// <p>
    /// While an interface is not strictly necessary, it allows us the
    /// flexibility of implementing our Merkle tree without a database backend.
    /// This could be useful, for example, in unit testing. See, for example,
    /// <a href="https://www.asp.net/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application">
    /// this article
    /// </a>.
    /// </p>
    /// </remarks>
    public interface IMerkleTreeContext {
        IContentBlobRepository ContentBlobRepository { get; }
        //ICorpusBlobRepository CorpusBlobRepository { get; }
        //INeuralNetBlobRepository NeuralNetBlobRepository { get; }
    }
}

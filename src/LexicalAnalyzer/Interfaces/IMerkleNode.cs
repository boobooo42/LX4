using LexicalAnalyzer.Models;

namespace LexicalAnalyzer.Interfaces {
    /* FIXME: I'm not sure if we really need this interface. --Jonathan */
    public interface IMerkleNode {
        MerkleHash Hash { get; }
        /// <summary>
        /// Returns true if the given Merkle node object is missing some of its
        /// contents, false otherwise.
        /// </summary>
        /// <remarks>
        /// <p>
        /// Many of the Merkle node objects, especially the corpus content
        /// objects, are much too large to fetch from the database all at once.
        /// The simple design of Dapper requires us to defer these expensive
        /// fetches manually. The assertion of this flag indicates that some or
        /// all of the content of the given Merkle node has not been fetched.
        /// </p>
        /// <p>
        /// Note that the hash is always fetched from the database, so we
        /// usually do not need the entire contents anyway.
        /// </p>
        /// </remarks>
        bool IsFlyweight { get; }
    }
}

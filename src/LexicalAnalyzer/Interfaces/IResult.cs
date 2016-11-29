namespace LexicalAnalyzer {
    public interface IResult {
        /// <summary>
        /// The fully-qualified class name of this result type.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// A JSON-encoded string representation of the data.
        /// </summary>
        string Data { get; }
    }
}

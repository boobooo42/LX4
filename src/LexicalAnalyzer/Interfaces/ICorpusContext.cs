namespace LexicalAnalyzer.Interfaces {
    public interface ICorpusContext {
        ICorpusRepository CorpusRepository { get; }
        ICorpusContentRepository CorpusContentRepository { get; }
    }
}

namespace LexicalAnalyzer.Models {
    public class Corpus {
        /* Attributes */
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Locked { get; set; }
        /* TODO: Add count of content entries */

        /* Constructors */
        public Corpus() {
            Id = -1;  /* Default constructed corpus objects start with an
                         invalid ID */
        }
    }
}

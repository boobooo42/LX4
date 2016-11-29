namespace LexicalAnalyzer.Models {
    public class RankFrequencyPair {
        /* Constructors */
        public RankFrequencyPair(
                string name,
                long rank,
                long frequency)
        {
            this.Name = name;
            this.Rank = rank;
            this.Frequency = frequency;
        }

        /* Properties */
        public string Name { get; set; }
        public long Rank { get; set; }
        public long Frequency { get; set; }
    }
}

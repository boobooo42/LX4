using System;
using System.Data.SqlTypes;

namespace LexicalAnalyzer.Models {
    public class CorpusContent {
        /* Accessors */
        public long Id { get; set; }
        public long CorpusId { get; set; }
        public string Hash { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        /* FIXME: Some of these data types are specific to MSSQL */
        public Guid ScraperGuid { get; set; }
        public string ScraperType { get; set; }
        public DateTime DownloadDate { get; set; }
        public string DownloadURL { get; set; }
        public byte[] Content { get; set; }
        public float Long { get; set; }
        public float Lat { get; set; }

        /* Constructors */
        public CorpusContent() {
            Id = -1;  /* Default constructed with an invalid ID */
        }
    }
}

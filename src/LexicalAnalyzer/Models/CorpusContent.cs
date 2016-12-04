using System;
using System.Collections.Generic;
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
        public SqlDateTime DownloadDate { get; set; }
        public string URL { get; set; }
        public byte[] Content { get; set; }
        public float Long { get; set; }
        public float Lat { get; set; }

        #region Twitter
        public long TweetID { get; set; }
        public string AuthorName { get; set; }
        public List<string> Hashtags { get; set; }        
        public string Language { get; set; }
        public string Source { get; set; }
        #endregion

        /* Constructors */
        public CorpusContent() {
            Id = -1;  /* Default constructed with an invalid ID */
        }
    }
}

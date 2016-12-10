using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace Dto
{
    public class CorpusContentDto
    {
            /* Accessors */
            public long Id { get; set; }
            public long CorpusId { get; set; }
            public string Hash { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            /* FIXME: Some of these data types are specific to MSSQL */
            public string ScraperGuid { get; set; }
            public string ScraperType { get; set; }
            public string DownloadDate { get; set; }
            public string URL { get; set; }
            public string Content { get; set; }
            public float Long { get; set; }
            public float Lat { get; set; }

            #region Twitter
            public long TweetID { get; set; }
            public string AuthorName { get; set; }
            public List<string> Hashtags { get; set; }
            public string Language { get; set; }
            public string Source { get; set; }
            #endregion

        
    }
}
using System;
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
        public Guid ScraperGuid { get; set; }
        public string ScraperType { get; set; }
        public DateTime DownloadDate { get; set; }
        public string DownloadURL { get; set; }
        public byte[] Content { get; set; }
    }
}
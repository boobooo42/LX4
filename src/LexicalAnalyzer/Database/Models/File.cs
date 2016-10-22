using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/* FIXME: This should be part of the LexicalAnalyzer.Models namespace or
 * similar */
namespace LexicalAnalyzer
{
    /* FIXME: This class name is much too close to System.IO.File */
    public class File
    {
        /* FIXME: These attributes do not need "File" in their name */
        public int FileID { get; set; }
        public string FileHash { get; set; }
        public string FileContents { get;set;}
        public string FileName { get; set; }
        public string DownloadDate { get; set; }
        public string DownloadURL { get; set; }

        

    }
}

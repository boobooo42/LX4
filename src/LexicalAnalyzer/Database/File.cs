using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexicalAnalyzer   
{
    public class File
    {
        public int FileID { get; set; }
        public string FileHash { get; set; }
        public string FileName { get; set; }
        public string DateDownloaded { get; set; }
        public string URLDownloaded { get; set; }

        

    }
}

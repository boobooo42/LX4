using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace LexicalAnalyzer.Models
{
    public class Tags
    {
        public string name { get; set; }
        public string zipball_url { get; set; }
        public string tarball_url { get; set; }
        public Commit commit { get; set; }
    }

    public class Commit
    {
        
       // public string sha { get; set; }
        public string url { get; set; }
    }
}
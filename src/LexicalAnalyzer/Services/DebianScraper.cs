using HtmlAgilityPack;
using LexicalAnalyzer.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace LexicalAnalyzer.Services
{
    public class DebianScraper : IScraper
    {
      /* Constants */
      private const string URL = "http://debian.osuosl.org/debian/pool/main/c/";
      private const string DownloadPath = "//a[contains(@href,'.deb')]";
      private const string LinkPath = "//a[@href]";

      private List<string> exeLinks;
      private List<string> urlList;
      private HtmlDocumentTree htmlDocumentTree;
      List<HtmlNode> DownLinkList;
      HtmlNodeCollection DownCollection;

      public static string ScraperName {
        get { return "debian"; }
      }

      public void Run() {
      }

      public float Progress {
        get {
          return 0.0f;
        }
      }
    }
}

using LexicalAnalyzer.Models;
using System.Collections.Generic;

namespace LexicalAnalyzer.Interfaces {
    public interface IScraperFactory {
        IEnumerable<ScraperType> ScraperTypes {
            get;
        }

        IScraper BuildScraper(string type);
    }
}

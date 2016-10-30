using System.Collections.Generic;
using System;

namespace LexicalAnalyzer.Interfaces {
    public interface IScraperService {
        IScraper CreateScraper(string type);
        IScraper GetScraper(Guid guid);
        IScraper GetScraper(string guid);
        bool RemoveScraper(Guid guid);
        bool RemoveScraper(string guid);
        IEnumerable<IScraper> Scrapers { get; }
        void StartScraper(Guid guid);
        void StartScraper(string guid);
        void PauseScraper(Guid guid);
        void PauseScraper(string guid);
    }
}

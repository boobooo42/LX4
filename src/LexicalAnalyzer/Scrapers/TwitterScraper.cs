using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreTweet.Core;


namespace LexicalAnalyzer.Scrapers
{
    public class TwitterScraper
    {
        void TwitterTest()
        {
            var key = "GzWUY0oTfH4AMZdnMqrm0wcde";
            var secret = "QfuQ7YgmLTmvQguuw3siKrwzPCiQ9EW7NleCvhxdRrjSKhfZww";
            var session = CoreTweet.OAuth2.GetTokenAsync(key, secret);

        }
    }
}

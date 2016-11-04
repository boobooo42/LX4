using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;

namespace LexicalAnalyzer.Scrapers
{
    public class TwitterScraper : IScraper
    {
        private Guid m_guid;
        private string m_status;
        private float m_progress;
        private int m_priority;
        private ICorpusContext m_context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <returns></returns>
        public TwitterScraper(ICorpusContext context)
        {
            m_guid = System.Guid.NewGuid();
            m_status = "init";
            m_progress = 0.0f;
            m_priority = 0;
            m_context = context;
        }

        #region Properties
        /// <summary>
        /// Gets guid
        /// </summary>
        /// <returns></returns>
        public Guid Guid
        {
            get
            {
                return m_guid;
            }
        }

        /// <summary>
        /// Gets display name--is hardcoded
        /// </summary>
        /// <returns></returns>
        public static string DisplayName
        {
            get { return "Twitter Scraper"; }
        }

        /// <summary>
        /// Gets description--is hardcoded
        /// </summary>
        /// <returns></returns>
        public static string Description
        {
            get
            {
                return
                    @"A scraper used for scraping tweets from Twitter.";
            }
        }

        /// <summary>
        /// Gets content type --is hardcoded
        /// </summary>
        /// <returns></returns>
        public static string ContentType
        {
            get
            {
                return "tweets";
            }
        }

        /// <summary>
        /// Gets status
        /// </summary>
        /// <returns></returns>
        public string Status
        {
            get
            {
                return m_status;
            }
            set
            {
                m_status = value;
            }
        }

        /// <summary>
        /// Gets progress
        /// </summary>
        /// <returns></returns>
        public float Progress
        {
            get
            {
                return m_progress;
            }
        }

        /// <summary>
        /// Gets priority
        /// </summary>
        /// <returns></returns>
        public int Priority
        {
            get
            {
                return m_priority;
            }
        }

        /// <summary>
        /// List of properties supported by TextScraper and their respective
        /// default values.
        /// </summary>
        public static IEnumerable<KeyValueProperty> DefaultProperties
        {
            get
            {
                var properties = new List<KeyValueProperty>();
                properties.Add(
                        new KeyValueProperty(
                            "timeout",  /* key */
                            "30",  /* defaultValue */
                            "seconds"  /* type */
                            ));
                properties.Add(
                        new KeyValueProperty(
                            "website",  /* key */
                            "https://twitter.com",  /* defaultValue */
                            "url"  /* type */
                            ));
                properties.Add(
                        new KeyValueProperty(
                            "tweetsToDownload", /* key */
                            "", /* defaultValue */
                            "urls" /* type */
                            ));
                return properties;
            }
        }

        /// <summary>
        /// Gets properties
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValueProperty> Properties
        {
            get; set;
        }
        #endregion

        /// <summary>
        /// Runs the text scraper
        /// </summary>
        /// <returns></returns>
        public void Run()
        {
            Debug.Assert(false);
            TwitterTest();
        }

        void TwitterTest()
        {
            var key = "GzWUY0oTfH4AMZdnMqrm0wcde";
            var secret = "QfuQ7YgmLTmvQguuw3siKrwzPCiQ9EW7NleCvhxdRrjSKhfZww";

            // Create a new set of credentials for the application.
            var appCredentials = new TwitterCredentials(key, secret);

            // Init the authentication process and store the related `AuthenticationContext`.
            var authenticationContext = AuthFlow.InitAuthentication(appCredentials);

            // Go to the URL so that Twitter authenticates the user and gives him a PIN code.
            Console.WriteLine(authenticationContext.AuthorizationURL);

            // Ask the user to enter the pin code given by Twitter
            Console.WriteLine("enter pin");
            var pinCode = Console.ReadLine();

            // With this pin code it is now possible to get the credentials back from Twitter
            var userCredentials = AuthFlow.CreateCredentialsFromVerifierCode(pinCode, authenticationContext);

            // Use the user credentials in your application
            Auth.SetCredentials(userCredentials);

            var stream = Stream.CreateSampleStream();
            stream.TweetReceived += (sender, args) =>
            {
                // Do what you want with the Tweet.
                Console.WriteLine(args.Tweet);
            };
            stream.StartStream();
        
    }
    }
}

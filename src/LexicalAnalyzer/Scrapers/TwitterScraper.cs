using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Models.Entities;

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
            string consumerKey = "GzWUY0oTfH4AMZdnMqrm0wcde";
            string consumerSecret = "QfuQ7YgmLTmvQguuw3siKrwzPCiQ9EW7NleCvhxdRrjSKhfZww";
            UserAuthentication(consumerKey, consumerSecret);
            FullTwitterSample();
        }

        void FullTwitterSample()
        {


            List<ITweet> tweetList = new List<ITweet>();

            // Enable Automatic RateLimit handling
            RateLimit.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;
            var stream = Stream.CreateSampleStream();
            stream.StallWarnings = true;
            stream.AddTweetLanguageFilter(LanguageFilter.English);
            stream.FilterLevel = Tweetinvi.Streaming.Parameters.StreamFilterLevel.Low;
            stream.TweetReceived += (sender, args) =>
            {
                // Do what you want with the Tweet.

                ITweet tweet = args.Tweet;
                try
                {
                    //  tweetList.Add(tweet);
                    Debug.WriteLine(tweet);
                    Console.WriteLine(tweet);

                    //if (tweetList.Count > 10)
                    //{
                    //    stream.StopStream();
                     //   foreach (ITweet tweet2 in tweetList)
                            ScraperUtilities.addCorpusContent("tweet", "tweet", this.Guid, 
                                this.GetType().FullName, tweet, this.m_context);
                   // }
                }

                catch { }

            };
            stream.StartStream();


        }

        void UserAuthentication(string consumerKey, string consumerSecret)
        {
            // Create a new set of credentials for the application.
            var appCredentials = new TwitterCredentials(consumerKey, consumerSecret);

            // Init the authentication process and store the related `AuthenticationContext`.
            var authenticationContext = AuthFlow.InitAuthentication(appCredentials);

            string authUrl = authenticationContext.AuthorizationURL;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start {authUrl}")); // Works ok on windows
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", authUrl);  // Works ok on linux
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", authUrl); // Not tested
            }


            // Ask the user to enter the pin code given by Twitter
            Debug.WriteLine("enter pin");
            Console.WriteLine("enter pin");
            //  var pinCode = "1"; //Now change pincode in immediate window
            var pinCode = Console.ReadLine();
            // Debug.Assert(false); 
            // With this pin code it is now possible to get the credentials back from Twitter
            var userCredentials = AuthFlow.CreateCredentialsFromVerifierCode(pinCode, authenticationContext);

            // Use the user credentials in your application
            Auth.SetCredentials(userCredentials);
        }

    }
}

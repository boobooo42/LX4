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
        private int m_downloadCount;
        private int m_downloadLimit;
        private Stopwatch m_timer;
        private int m_timeLimit;
        private List<KeyValueProperty> m_properties;
        private bool m_authorized;

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
            m_downloadCount = 0;
            m_downloadLimit = 0;
            m_timer = new Stopwatch();
            m_timeLimit = 0;
        }

        #region Properties
        /// <summary>
        /// Gets the scraper type
        /// </summary>
        /// <returns></returns>
        public string Type
        {
            get
            {
                return this.GetType().FullName;
            }
        }

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
        public string DName { get { return "Twitter Scraper"; } }
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
        public string Desc
        {
            get { return @"A scraper used for scraping tweets from Twitter."; }
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

        public bool Authorized
        {
            get { return m_authorized; }
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

        public int DownloadCount
        {
            get
            {
                return m_downloadCount;
            }
        }
        public int DownloadLimit
        {
            get
            {
                return m_downloadLimit;
            }

            set
            {
                m_downloadLimit = value;
            }
        }

        public Stopwatch Timer
        {
            get
            {
                return m_timer;
            }
        }

        public int TimeLimit
        {
            get
            {
                return m_timeLimit;
            }

            set
            {
                m_timeLimit = value;
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
                            "timelimit",  /* key */
                            "",  /* defaultValue */
                            "seconds"  /* type */
                            ));
                properties.Add(
                        new KeyValueProperty(
                            "downloadlimit",  /* key */
                            "",  /* defaultValue */
                            "items"  /* type */
                            ));
                properties.Add(
                        new KeyValueProperty(
                            "website",  /* key */
                            "https://twitter.com",  /* defaultValue */
                            "url"  /* type */
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
            get { return m_properties; }
            set
            {
                foreach (var property in value)
                {
                    if (property.Key == "timelimit")
                        TimeLimit = int.Parse(property.Value);
                    else if (property.Key == "downloadlimit")
                        DownloadLimit = int.Parse(property.Value);
                }
                m_properties = new List<KeyValueProperty>(value);
            }
        }
        #endregion

        /// <summary>
        /// Runs the twitter scraper
        /// </summary>
        /// <returns></returns>
        public void Run()
        {
            m_timer.Reset();
            m_timer.Start();
            TwitterTest();
        }

        public string TwitterTest()
        {
            Debug.Assert(false);
            string consumerKey = "GzWUY0oTfH4AMZdnMqrm0wcde";
            string consumerSecret = "QfuQ7YgmLTmvQguuw3siKrwzPCiQ9EW7NleCvhxdRrjSKhfZww";
            FullTwitterSample();
            return UserAuthentication(consumerKey, consumerSecret);
            
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
            stream.StartStream();
            m_downloadCount = 0;
            m_timer.Reset();
            bool downloadLimitReached = downloadStop();
            bool timeLimitReached = timeStop();
            m_timer.Start();
            while (!downloadLimitReached && !timeLimitReached)
            {
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
                        ScraperUtilities.addCorpusContent("Twitter", "tweet", this.Guid,
                            this.GetType().FullName, tweet, this.m_context);
                        // }
                        m_downloadCount++;
                        m_progress = (float)m_downloadCount / m_downloadLimit;
                    }
                    catch { }
                };
                downloadLimitReached = downloadStop();
                timeLimitReached = timeStop();
            }
            m_status = "stopped on ";
            if (downloadLimitReached && timeLimitReached)
                m_status += "downloads, time";
            else if (downloadLimitReached)
                m_status += "downloads";
            else if (timeLimitReached)
                m_status += "time";
        }


        IAuthenticationContext authenticationContext;
        public string UserAuthentication(string consumerKey, string consumerSecret)
        {
            // Create a new set of credentials for the application.
            var appCredentials = new TwitterCredentials(consumerKey, consumerSecret);

            // Init the authentication process and store the related `AuthenticationContext`.
            authenticationContext = AuthFlow.InitAuthentication(appCredentials);

            return authenticationContext.AuthorizationURL;

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
            // Go to the URL so that Twitter authenticates the user and gives him a PIN code.


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


        public void FinishUserAuthentication(string pinCode)
        {
            // With this pin code it is now possible to get the credentials back from Twitter
            var userCredentials = AuthFlow.CreateCredentialsFromVerifierCode(pinCode, authenticationContext);

            // Use the user credentials in your application
            Auth.SetCredentials(userCredentials);
            m_authorized = true;
        }

        public bool downloadStop()
        {
            if (DownloadCount >= DownloadLimit)
                return true;
            else
                return false;
        }

        public bool timeStop()
        {
            if (m_timer.ElapsedMilliseconds >= TimeLimit * 1000)
            {
                m_timer.Reset();
                return true;
            }
            else
                return false;
        }
    }
}

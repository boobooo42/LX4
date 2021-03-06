﻿using LexicalAnalyzer.Interfaces;
using LexicalAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
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
        private int m_downloadCount;
        private int m_downloadLimit;
        private Stopwatch m_timer;
        private int m_timeLimit;
        private int m_corpusId;
        private string m_userGivenName;
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
        public string TypeName { get { return "Twitter Scraper"; } }
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

        public string UserGivenName
        {
            get
            {
                return m_userGivenName;
            }

            set
            {
                m_userGivenName = value;
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
                    else if (property.Key == "UserGivenName")
                        UserGivenName = property.Value;
                    else if (property.Key == "corpus")
                        CorpusId = int.Parse(property.Value);
                }
                m_properties = new List<KeyValueProperty>(value);
            }
        }

        public int CorpusId
        {
            get
            {
                return m_corpusId;
            }

            set
            {
                m_corpusId = value;
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
            StartTwitterStream();
        }

        /// <summary>
        /// deprecated run of twitter scraper
        /// </summary>
        /// <returns></returns>
        public string TwitterTest()
        {
            Debug.Assert(false);
            string consumerKey = "GzWUY0oTfH4AMZdnMqrm0wcde";
            string consumerSecret = "QfuQ7YgmLTmvQguuw3siKrwzPCiQ9EW7NleCvhxdRrjSKhfZww";
            return UserAuthentication(consumerKey, consumerSecret);

        }

        /// <summary>
        /// sets up conditions for twitter stream and runs the stream
        /// </summary>
        void StartTwitterStream()
        {
            List<ITweet> tweetList = new List<ITweet>();

            // Enable Automatic RateLimit handling
            RateLimit.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;
            var stream = Stream.CreateSampleStream();
            stream.StallWarnings = true;
            stream.AddTweetLanguageFilter(LanguageFilter.English);
            stream.FilterLevel = Tweetinvi.Streaming.Parameters.StreamFilterLevel.None;
            bool auth = Authorized;
            m_downloadCount = 0;
            m_timer.Reset();
            //bool downloadLimitReached = downloadStop();
            //bool timeLimitReached = timeStop();
            m_timer.Start();

            //have to add this before startign the stream, tells us what to do
            //each time we recieve a tweet
            stream.TweetReceived += (sender, args) =>
            {

                ITweet tweet = args.Tweet;
                //Debug.Assert(false);
                try
                {

                    Debug.WriteLine(tweet);
                    Console.WriteLine(tweet);
                    string title = getTweetName(tweet);
                    ScraperUtilities.addCorpusContent(tweet.Text, "tweet", this.Guid,
                    this.GetType().FullName, tweet, this.m_context, m_corpusId);
                    m_downloadCount++;
                    m_progress = (float)m_downloadCount / m_downloadLimit;
                    if (timeStop() || downloadStop())
                        StopTwitterStream(stream);
                }
                catch 
                {
                    StopTwitterStream(stream);
                }
                
            };

            //start the stream, now that we know what to do with it
            if (m_authorized)
                stream.StartStream();
            else
            {
                m_status = "No Twitter Authorization";
                    //string consumerKey = "GzWUY0oTfH4AMZdnMqrm0wcde";
                    //string consumerSecret = "QfuQ7YgmLTmvQguuw3siKrwzPCiQ9EW7NleCvhxdRrjSKhfZww";
                    //UserAuthentication(consumerKey, consumerSecret);
                    //StartTwitterStream();                
            }
        }

        /// <summary>
        /// stops the current twitter stream 
        /// and produces and AAR
        /// </summary>
        /// <param name="stream"></param>
        void StopTwitterStream(Tweetinvi.Streaming.ISampleStream stream)
        {
            stream.StopStream();
            if (downloadStop() && timeStop())
                m_status = ScraperUtilities.SCRAPER_STATUS_TIME_AND_DOWNLOAD_LIMIT_REACHED ;
            else if (downloadStop())
                m_status = ScraperUtilities.SCRAPER_STATUS_DOWNLOAD_LIMIT_REACHED;
            else if (timeStop())
            {
                m_timer.Reset();
                m_status = ScraperUtilities.SCRAPER_STATUS_TIME_LIMIT_REACHED;
            }
            else m_status = ScraperUtilities.SCRAPER_STATUS_APPLICATION_ERROR;
        }

        IAuthenticationContext authenticationContext;
        public string UserAuthentication(string consumerKey, string consumerSecret)
        {
            // Create a new set of credentials for the application.
            var appCredentials = new TwitterCredentials(consumerKey, consumerSecret);

            // Init the authentication process and store the related `AuthenticationContext`.
            authenticationContext = AuthFlow.InitAuthentication(appCredentials);



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
            m_authorized = true;
            return authenticationContext.AuthorizationURL;
        }

        public string UserAuthentication()
        {
            string consumerKey = "GzWUY0oTfH4AMZdnMqrm0wcde";
            string consumerSecret = "QfuQ7YgmLTmvQguuw3siKrwzPCiQ9EW7NleCvhxdRrjSKhfZww";
            // Create a new set of credentials for the application.
            var appCredentials = new TwitterCredentials(consumerKey, consumerSecret);
           
            // Init the authentication process and store the related `AuthenticationContext`.
            authenticationContext = AuthFlow.InitAuthentication(appCredentials);
            return authenticationContext.AuthorizationURL;
            
        }


        public void FinishUserAuthentication(string pinCode)
        {
            // With this pin code it is now possible to get the credentials back from Twitter
            var userCredentials = AuthFlow.CreateCredentialsFromVerifierCode(pinCode, authenticationContext);

            // Use the user credentials in your application
            Auth.SetCredentials(userCredentials);
            m_authorized = true;
            //FullTwitterSample();
        }

        /// <summary>
        /// gives us a name for our tweet
        /// </summary>
        /// <param name="tweet"></param>
        /// <returns></returns>
        string getTweetName(ITweet tweet)
        {

            //here we are getting the link to the next page using  a regex split

            string title = "Tweet";
            if (tweet.Text.Length < 31)
            {
                title = tweet.Text;

            }
            else title = tweet.Text.Substring(0,31);

            return title;
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
            return (m_timer.ElapsedMilliseconds >= TimeLimit * 1000);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;

namespace LexicalAnalyzer.Scrapers
{
    public class TwitterScraper
    {
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

using System;
using System.Linq;
using Codeplex.OAuth;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif

namespace Utakotoha.Model
{
    public class TwitterRequest
    {
        const string ConsumerKey = "zrYrxJszZfUIfAXkDJVAg";
        const string ConsumerSecret = "NuNJFqy0prq4ptnyfOGImK0nOHcCvyzOG69zg8nd9I"; // secret secret...

        private readonly AccessToken accessToken;

        public TwitterRequest(AccessToken accessToken)
        {
            this.accessToken = accessToken;
        }

        public static IObservable<RequestToken> GetRequestToken()
        {
            return new OAuthAuthorizer(ConsumerKey, ConsumerSecret)
                .GetRequestToken("http://twitter.com/oauth/request_token")
                .Select(res => res.Token);
        }

        public static string GetAuthorizeUrl(RequestToken token)
        {
            return new OAuthAuthorizer(ConsumerKey, ConsumerSecret)
                .BuildAuthorizeUrl("http://twitter.com/oauth/authorize", token);
        }

        public static IObservable<TwitterCredential> GetAccessToken(RequestToken requestToken, string pincode)
        {
            return new OAuthAuthorizer(ConsumerKey, ConsumerSecret)
                .GetAccessToken("http://twitter.com/oauth/access_token", requestToken, pincode)
                .Select(res => new TwitterCredential(res.ExtraData["screen_name"].First(), res.Token));
        }

        public IObservable<string> Post(string postText)
        {
            var client = new OAuthClient(ConsumerKey, ConsumerSecret, accessToken)
            {
                MethodType = MethodType.Post,
                Url = "http://api.twitter.com/1/statuses/update.json",
                Parameters = { { "status", postText } }
            };
            return client.GetResponseText();
        }
    }
}
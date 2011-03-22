using Codeplex.OAuth;

namespace Utakotoha
{
    public class TwitterCredential
    {
        public AccessToken OAuthToken { get; private set; }
        public string TwitterUserId { get; private set; }

        public TwitterCredential(string userid, AccessToken token)
        {
            this.TwitterUserId = userid;
            this.OAuthToken = token;
        }
    }
}
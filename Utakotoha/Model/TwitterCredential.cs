﻿using Codeplex.OAuth;
using System.Runtime.Serialization;

namespace Utakotoha.Model
{
    public class TwitterCredential
    {
        public AccessToken AccessToken { get; private set; }
        public string ScreenName { get; private set; }

        public TwitterCredential(string screenName, AccessToken accessToken)
        {
            this.ScreenName = screenName;
            this.AccessToken = accessToken;
        }
    }
}
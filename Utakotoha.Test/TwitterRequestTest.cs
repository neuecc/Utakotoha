using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;
using Sgml;
using System.Xml.Linq;
using System.Diagnostics;

namespace Utakotoha.Test
{
    [TestClass]
    public class TwitterRequestTest
    {
        [TestMethod]
        [Timeout(3000)]
        public void GetRequestToken()
        {
            var token = TwitterRequest.GetRequestToken().First();

            token.Is(t => t.Key != "" && t.Secret != "");
        }

        // browser manual input...
        [TestMethod]
        [Ignore]
        public void Credential()
        {
            System.Net.ServicePointManager.Expect100Continue = false; // post

            var token = TwitterRequest.GetRequestToken().First();
            Process.Start(TwitterRequest.GetAuthorizeUrl(token));

            var pincode = ""; // debug stop.

            var credential = TwitterRequest.GetAccessToken(token, pincode).First();
            credential.ScreenName.Is("neuecc");
            credential.AccessToken.Is(t => t.Key != "" && t.Secret != "");

            // post test
            var result = new TwitterRequest(credential.AccessToken).Post("testてすと").First();
            result.Is(s => s.Contains("test"));
        }
    }
}

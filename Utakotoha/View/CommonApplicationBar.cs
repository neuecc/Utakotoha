using System;
using System.Linq;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Reactive;
using Utakotoha.Model;

namespace Utakotoha.View
{
    public static class CommonApplicationBar
    {
        public static ApplicationBar Create()
        {
            var applicationBar = new ApplicationBar()
            {
                IsVisible = true,
                IsMenuEnabled = true,
                Opacity = 0.9
            };

            var search = new ApplicationBarIconButton(new Uri("/Images/button1.png", UriKind.Relative)) { Text = "search", IsEnabled = true };
            search.ClickAsObservable().Subscribe(_ =>
            {
            });

            var tweet = new ApplicationBarIconButton(new Uri("/Images/button1.png", UriKind.Relative)) { Text = "tweet", IsEnabled = true };
            tweet.ClickAsObservable().Subscribe(_ =>
            {
                // TODO:no music no life.
                var token = Settings.Load().TwitterCredential.AccessToken;
                new TwitterRequest(token).Post("test").Subscribe();
            });


            var amazon = new ApplicationBarIconButton(new Uri("/Images/button1.png", UriKind.Relative)) { Text = "amazon", IsEnabled = true };

            var settings = new ApplicationBarMenuItem("settings") { IsEnabled = true };
            var about = new ApplicationBarMenuItem("about") { IsEnabled = true };

            Array.ForEach(new[] { search, tweet, amazon }, o => applicationBar.Buttons.Add(o));
            Array.ForEach(new[] { settings, about }, o => applicationBar.MenuItems.Add(o));

            return applicationBar;
        }
    }
}
using System;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Text.RegularExpressions;

#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif

namespace Utakotoha
{
    public partial class TwitterAuth : PhoneApplicationPage
    {
        public TwitterAuth()
        {
            InitializeComponent();

            this.LoadedAsObservable()
                .Take(1)
                .SelectMany(_ => TwitterRequest.GetRequestToken())
                .Report(t =>
                {
                    var uri = new Uri(TwitterRequest.GetAuthorizeUrl(t));
                    webBrowser1.Navigate(uri);
                })
                .SelectMany(_ => webBrowser1.NavigatedAsObservable(), (t, _) => t)
                .Skip(1)
                .Report(_ =>
                {
                    var html = webBrowser1.SaveToString();
                    PinTextBox.Text = Regex.Match(html, "<DIV id=oauth_pin>(.+?)</DIV>").Groups[1].Value;
                })
                .SelectMany(_ => OKButton.ClickAsObservable(), (t, _) => t)
                .SelectMany(t => TwitterRequest.GetAccessToken(t, PinTextBox.Text.Trim()))
                .ObserveOnDispatcher()
                .Finally(() => NavigationService.GoBack())
                .Subscribe(c =>
                {
                    var settings = Settings.Load();
                    settings.TwitterCredential = c;
                    settings.Save();

                    NavigationService.GoBack();
                },
                e => MessageBox.Show("Error:" + e.ToString()));
        }
    }
}
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif
using System;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Text.RegularExpressions;
using Utakotoha.Model;

namespace Utakotoha.View
{
    public partial class TwitterAuth : PhoneApplicationPage
    {
        private void SwitchProgress(bool isOnProgress)
        {
            MainProgressBar.IsIndeterminate = isOnProgress;
        }

        public TwitterAuth()
        {
            InitializeComponent();

            this.LoadedAsObservable()
                .Take(1)
                .Do(_ => SwitchProgress(true))
                .SelectMany(_ => TwitterRequest.GetRequestToken())
                .Report(t =>
                {
                    var uri = new Uri(TwitterRequest.GetAuthorizeUrl(t));
                    SwitchProgress(false);
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
                .Report(_ => SwitchProgress(true))
                .SelectMany(t => TwitterRequest.GetAccessToken(t, PinTextBox.Text.Trim()))
                .ObserveOnDispatcher()
                .Finally(() => NavigationService.GoBack())
                .Subscribe(c =>
                {
                    var settings = Settings.Default;
                    settings.TwitterCredential = c;
                    settings.Save();
                    NavigationService.GoBack();
                },
                e => MessageBox.Show("AuthorizeError"));
        }
    }
}
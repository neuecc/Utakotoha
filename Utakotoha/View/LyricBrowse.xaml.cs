using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System.Windows.Navigation;
using System.Text.RegularExpressions;
using System.IO.IsolatedStorage;
using System.Diagnostics;
using Microsoft.Phone.Shell;
using Utakotoha.Model;
using Utakotoha.Model.Bing;

namespace Utakotoha.View
{
    public partial class LyricBrowse : PhoneApplicationPage
    {
        const string GooUri = "http://music.goo.ne.jp";
        const string GooLyricUri = "http://music.goo.ne.jp/lyric";

        public LyricBrowse()
        {
            InitializeComponent();
            ApplicationBar = CommonApplicationBar.Create();
        }

        CompositeDisposable disposables = new CompositeDisposable();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            lyricBrowser.NavigatedAsObservable()
                .Where(ev => ev.EventArgs.Uri.AbsoluteUri.Contains(GooLyricUri))
                .SelectMany(ev =>
                {
                    // polling when can get attribute
                    return Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(3))
                        .ObserveOnDispatcher()
                        .Select(_ => (ev.Sender as WebBrowser).SaveToString())
                        .Select(s => Regex.Match(s, @"s.setAttribute\('src', '(.+?)'"))
                        .Where(m => m.Success)
                        .Take(1);
                })
                .Select(m => WebRequest.Create(GooUri + m.Groups[1].Value).DownloadStringAsync())
                .Switch()
                .Select(s => Regex.Replace(s.Trim(), @"^draw\(|\);$", ""))
                .Where(s => !string.IsNullOrEmpty(s))
                .ObserveOnDispatcher()
                .Subscribe(jsonArray =>
                {
                    // insert json array to html
                    lyricBrowser.InvokeScript("eval", @"
                        var array = " + jsonArray + @";
                        var sb = [];
                        for(var i = 0; i < array.length; i++) sb.push(array[i]);
                        document.getElementById('lyric_area').innerHTML = sb.join('<br />')");
                })
                .Tap(disposables.Add);

            MediaPlayerStatus.PlayingSongChanged()
               .Where(_ => Settings.Load().IsAutoSearchWhenMusicChanged)
               .SelectMany(s => s.SearchLyric(), (newsong, searchresult) => new { newsong, searchresult })
               .ObserveOnDispatcher()
               .Subscribe(a =>
               {
                   IsolatedStorageSettings.ApplicationSettings[Key.PlayingSong] = a.newsong;
                   IsolatedStorageSettings.ApplicationSettings[Key.SongSearchResult] = a.searchresult;
                   IsolatedStorageSettings.ApplicationSettings.Save();
                   PageTitle.Text = a.newsong.Title + " - " + a.newsong.Artist;
                   lyricBrowser.Navigate(new Uri(a.searchresult.Url));
               })
               .Tap(disposables.Add);

            var song = (Song)IsolatedStorageSettings.ApplicationSettings[Key.PlayingSong];
            var searchResult = (SearchWebResult)IsolatedStorageSettings.ApplicationSettings[Key.SongSearchResult];

            PageTitle.Text = song.Title + " - " + song.Artist;
            Dispatcher.BeginInvoke(() => lyricBrowser.Navigate(new Uri(searchResult.Url)));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            disposables.Dispose();
        }
    }
}
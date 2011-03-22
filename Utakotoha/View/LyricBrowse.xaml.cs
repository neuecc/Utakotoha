using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System.Windows.Navigation;
using System.Text.RegularExpressions;
using System.IO.IsolatedStorage;
using System.Diagnostics;

namespace Utakotoha
{
    public partial class LyricBrowse : PhoneApplicationPage
    {
        const string GooUri = "http://music.goo.ne.jp";
        const string GooLyricUri = "http://music.goo.ne.jp/lyric";

        public LyricBrowse()
        {
            InitializeComponent();
            ApplicationBar = CommonApplicationBar.Create();

            lyricBrowser.NavigatedAsObservable()
                .Where(e => e.EventArgs.Uri.AbsoluteUri.Contains(GooLyricUri))
                .SelectMany(e =>
                {
                    // 属性が取れるまで3秒毎にポーリングでチェック
                    return Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(3))
                        .ObserveOnDispatcher()
                        .Select(_ => (e.Sender as WebBrowser).SaveToString());
                })
                .Select(s => Regex.Match(s, @"s.setAttribute\('src', '(.+?)'"))
                .Where(m => m.Success)
                .Take(1)
                .Select(m => WebRequest.Create(GooUri + m.Groups[1].Value).DownloadStringAsync())
                .Switch()
                .Select(s => Regex.Replace(s.Trim(), @"^draw\(|\);$", ""))
                .Where(s => !string.IsNullOrEmpty(s))
                .ObserveOnDispatcher()
                .Subscribe(jsonArray =>
                {
                    // Json配列を突っ込んで無理やりはめ込み合成する
                    lyricBrowser.InvokeScript("eval", @"
                        var array = " + jsonArray + @";
                        var sb = [];
                        for(var i = 0; i < array.length; i++) sb.push(array[i]);
                        document.getElementById('lyric_area').innerHTML = sb.join('<br />')");
                });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string lastsong;
            string lastsearch;
            if (!NavigationContext.QueryString.TryGetValue("song", out lastsong)
                || !NavigationContext.QueryString.TryGetValue("url", out lastsearch))
            {
                return;
            }
            var song = (Song)IsolatedStorageSettings.ApplicationSettings[lastsong];
            var searchResult = (SearchResult)IsolatedStorageSettings.ApplicationSettings[lastsearch];

            PageTitle.Text = song.Title + " - " + song.Artist;
            Dispatcher.BeginInvoke(() => lyricBrowser.Navigate(new Uri(searchResult.Url)));
        }
    }
}
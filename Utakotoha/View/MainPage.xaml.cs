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
using System.IO;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;
using Microsoft.Phone.Reactive;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Text;
using System.Windows.Threading;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Media;
using System.Reflection;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using Utakotoha.Model;
using Utakotoha.Model.Bing;
using System.Collections.ObjectModel;

namespace Utakotoha.View
{
    public partial class MainPage : PhoneApplicationPage
    {
        const string GooUri = "http://music.goo.ne.jp";
        const string GooLyricUri = "http://music.goo.ne.jp/lyric";

        ObservableCollection<SearchWebResult> searchResults = new ObservableCollection<SearchWebResult>();

        public MainPage()
        {
            InitializeComponent();

            ApplicationBar = CommonApplicationBar.Create();
            SearchListBox.ItemsSource = searchResults;


            var sr = new SearchWebResult { Title = "俺ら東京さ行ぐだ", Url = "http://music.goo.ne.jp/lyric/LYRUTND1127/index.html" };
            searchResults.Add(sr);

            SearchListBox.SelectionChangedAsObservable()
                .Subscribe(e =>
                {
                    var r = e.EventArgs.AddedItems.Cast<SearchWebResult>().First();
                    LyricBrowser.Navigate(new Uri(r.Url));
                    MainPivot.SelectedIndex = 1;
                });
        }

        CompositeDisposable disposables = new CompositeDisposable();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Observable.Merge(
                    MediaPlayerStatus.PlayingSongChanged(),
                    MediaPlayerStatus.PlayingSongActive())
                .Where(_ => Settings.Load().IsAutoSearchWhenMusicChanged)
                .Report(_ =>
                {
                    searchResults.Clear();
                })
                // .SelectMany(a => a.SearchLyric())
                .SelectMany(a => a.SearchFromArtist())
                .ObserveOnDispatcher()
                .Subscribe(searchResults.Add)
                .Tap(disposables.Add);

            //.SelectMany(s => s.SearchLyric(), (song, searchresult) => new { song, searchresult })
            //.ObserveOnDispatcher()
            //.Subscribe(a =>
            //{
            //    IsolatedStorageSettings.ApplicationSettings[Key.PlayingSong] = a.song;
            //    IsolatedStorageSettings.ApplicationSettings[Key.SongSearchResult] = a.searchresult;
            //    IsolatedStorageSettings.ApplicationSettings.Save();
            //    NavigationService.Navigate(new Uri("/View/LyricBrowse.xaml", UriKind.Relative));
            //});





            LyricBrowser.NavigatedAsObservable()
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
                     LyricBrowser.InvokeScript("eval", @"
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
                       LyricBrowser.Navigate(new Uri(a.searchresult.Url));
                   })
                   .Tap(disposables.Add);

                //var song = (Utakotoha.Model.Song)IsolatedStorageSettings.ApplicationSettings[Key.PlayingSong];
                //var searchResult = (SearchWebResult)IsolatedStorageSettings.ApplicationSettings[Key.SongSearchResult];

                //PageTitle.Text = song.Title + " - " + song.Artist;
                //Dispatcher.BeginInvoke(() => LyricBrowser.Navigate(new Uri(searchResult.Url)));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            disposables.Dispose();
        }
    }
}
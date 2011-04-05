using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using Utakotoha.Model;
using Utakotoha.Model.Bing;

namespace Utakotoha.View
{
    public partial class MainPage : PhoneApplicationPage
    {
        public class SearchSource
        {
            public string DisplayText { get; set; }
            public Func<Song, IObservable<SearchWebResult>> SearchMethod { get; set; }
        }

        const string GooUri = "http://music.goo.ne.jp";
        const string GooLyricUri = "http://music.goo.ne.jp/lyric";
        readonly SearchWebResult NotFound = new SearchWebResult { Title = "Not Found", Description = "Not Found or Not Seacrh Yet" };
        readonly SearchWebResult Error = new SearchWebResult { Title = "Error", Description = "Network Error" };

        ObservableCollection<SearchWebResult> searchResults = new ObservableCollection<SearchWebResult>();
        List<IDisposable> disposables = new List<IDisposable>();

        public MainPage()
        {
            InitializeComponent();

            SearchListBox.ItemsSource = searchResults;
            SearchListPicker.ItemsSource = new[]
            {
                new SearchSource{DisplayText = "from playing artist & song title", SearchMethod = o => o.SearchLyric()},
                new SearchSource{DisplayText = "from playing song title", SearchMethod = o => o.SearchFromTitle()},
                new SearchSource{DisplayText = "from playing artist", SearchMethod = o => o.SearchFromArtist()}
            };

            searchResults.Add(NotFound);
        }

        private void SwitchProgress(bool isOnProgress)
        {
            MainProgressBar.IsIndeterminate = isOnProgress;
        }

        protected override void OnNavigatedTo(NavigationEventArgs navArgs)
        {
            base.OnNavigatedTo(navArgs);

            // Tap ListBox
            SearchListBox.SelectionChangedAsObservable()
                .Where(e => e.EventArgs.AddedItems.Count > 0)
                .Select(e => e.EventArgs.AddedItems.Cast<SearchWebResult>().First())
                .Where(r => !string.IsNullOrEmpty(r.Url))
                .Subscribe(r =>
                {
                    LyricPivotItem.Header = r.Title;
                    LyricBrowser.Navigate(new Uri(r.Url));
                    MainPivot.SelectedIndex = 1;
                })
                .Tap(disposables.Add);

            // Search Lyrics
            Observable.Merge(
                    MediaPlayerStatus.PlayingSongChanged(),
                    MediaPlayerStatus.PlayingSongActive())
                .Where(_ => Settings.Default.IsAutoSearchWhenMusicChanged)
                .Merge(ApplicationBar.Buttons.Cast<ApplicationBarIconButton>()
                    .First(b => b.Text == "search")
                    .ClickAsObservable()
                    .Select(_ => MediaPlayerStatus.FromCurrent().ActiveSong)
                    .Where(s => s != null)
                    .Select(s => new Song(s.Artist.Name, s.Name))
                    .Report(_ => MainPivot.SelectedIndex = 0))
                .ObserveOnDispatcher()
                .Do(_ =>
                {
                    SwitchProgress(true);
                    searchResults.Clear();
                })
                .SelectMany(s => (SearchListPicker.SelectedItem as SearchSource)
                    .SearchMethod(s)
                    .ObserveOnDispatcher()
                    .Finally(() => SwitchProgress(false))
                    .Publish(xs =>
                    {
                        // normal
                        xs.Subscribe(searchResults.Add);
                        // not found
                        xs.Any()
                            .Where(found => !found)
                            .Subscribe(_ => searchResults.Add(NotFound));
                        // I'm Feeling Lucky
                        xs.Count()
                            .Any()
                            .Where(found => found && Settings.Default.IsAutoSelectFirstMatch)
                            .Subscribe(_ => SearchListBox.SelectedIndex = 0);

                        return xs;
                    }))
                .Catch((Exception ex) =>
                {
                    searchResults.Add(Error);
                    return Observable.Empty<SearchWebResult>();
                })
                .Repeat()
                .Subscribe()
                .Tap(disposables.Add);

            // Show Lyric to Browser
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

                     // change background color
                     if (ApplicationThemeManager.Current == ApplicationTheme.Dark)
                     {
                         LyricBrowser.InvokeScript("eval", @"
                            var divs = document.getElementsByTagName('div');
                            for(var i = 0, len = divs.length; i < len; i++)
                            {
                                divs[i].style.color = 'white';
                                divs[i].style.backgroundColor = 'black';
                            }
                         ");
                     }
                 }, e => MessageBox.Show("Browser Error"))
                 .Tap(disposables.Add);

            // Tweet
            ApplicationBar.Buttons.Cast<ApplicationBarIconButton>()
                .First(b => b.Text == "tweet")
                .ClickAsObservable()
                .Select(_ => MediaPlayerStatus.FromCurrent())
                .Subscribe(song =>
                {
                    var credential = Settings.Default.TwitterCredential;
                    if (credential == null)
                    {
                        MessageBox.Show("at first, authorize twitter id");
                        return;
                    }
                    if (song.ActiveSong == null)
                    {
                        MessageBox.Show("at first, play song");
                        return;
                    }

                    var hash = " #NowPlaying";
                    var songconcat = song.ActiveSong.Name + " - " + song.ActiveSong.Artist;
                    var msg = (songconcat.Length > 140) ? songconcat.Substring(0, 140)
                        : ((songconcat + hash).Length > 140) ? songconcat
                        : songconcat + hash;

                    var ok = MessageBox.Show("Post:" + msg, "", MessageBoxButton.OKCancel);
                    if (ok != MessageBoxResult.OK) return;

                    SwitchProgress(true);
                    new TwitterRequest(credential.AccessToken)
                        .Post(msg)
                        .ObserveOnDispatcher()
                        .Finally(() => SwitchProgress(false))
                        .Subscribe(_ => { }, e => { MessageBox.Show("Post Failed"); });
                })
                .Tap(disposables.Add);

            // Settings
            ApplicationBar.MenuItems.Cast<ApplicationBarMenuItem>()
                .First(b => b.Text == "settings")
                .ClickAsObservable()
                .Subscribe(_ => NavigationService.Navigate(new Uri("/View/SettingsPage.xaml", UriKind.Relative)))
                .Tap(disposables.Add);

            // About
            ApplicationBar.MenuItems.Cast<ApplicationBarMenuItem>()
                .First(b => b.Text == "about/help")
                .ClickAsObservable()
                .Subscribe(_ => NavigationService.Navigate(new Uri("/View/AboutPage.xaml", UriKind.Relative)))
                .Tap(disposables.Add);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            disposables.ForEach(d => d.Dispose());
            disposables.Clear();
        }
    }
}
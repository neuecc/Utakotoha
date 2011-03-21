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

namespace Utakotoha
{
    public partial class MainPage : PhoneApplicationPage
    {
        const string GooUri = "http://music.goo.ne.jp";
        const string GooHost = "music.goo.ne.jp";
        const string GooLyricUri = "http://music.goo.ne.jp/lyric";

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            Observable.FromEvent<EventArgs>(h => MediaPlayer.ActiveSongChanged += h, h => MediaPlayer.ActiveSongChanged -= h)
                .StartWith(default(IEvent<EventArgs>))
                .Throttle(TimeSpan.FromSeconds(3))
                .ObserveOnDispatcher()
                .Select(_ => (MediaPlayer.State == MediaState.Playing && MediaPlayer.Queue.ActiveSong != null)
                        ? new { Artist = MediaPlayer.Queue.ActiveSong.Artist.Name, Song = MediaPlayer.Queue.ActiveSong.Name }
                        : null)
                .Where(a => a != null)
                .Select(a => string.Format("site:{0} {1} {2}", GooLyricUri, a.Artist, a.Song))
                .Subscribe(q => LyricBrowser.Navigate(new Uri("http://google.co.jp/search?num=10&q=" + Uri.EscapeUriString(q))));

//            Observable.FromEvent<NavigationEventArgs>(h => LyricBrowser.Navigated += h, h => LyricBrowser.Navigated -= h)
//                .Where(e => !e.EventArgs.Uri.Host.Contains(GooHost))
//                .Select(e => (e.Sender as WebBrowser).SaveToString())
//                .Select(s =>
//                    (string)LyricBrowser.InvokeScript("eval", @"
//                        var array = document.getElementsByTagName('a');
//                        var result = [];
//                        for(var i = 0; i < array.length; i++)
//                        {
//                            var item = array[i];
//                            if(   item.href.indexOf('index.html') != -1
//                               && item.innerText.indexOf('歌詞情報 - goo 音楽') != -1)
//                               //&& item.innerText.indexOf('" + TempArtistName + @"') != -1
//                               //&& item.innerText.indexOf ('" + TempMusicName + @"') != -1)
//                            {
//                                result.push(item);
//                            }
//                        };
//                        
//                        (result.length != 0) ? result[0].href : ''")
//                )
//                .Catch(Observable.Return(""))
//                .Repeat()
//                .Subscribe(s =>
//                {
//                    if (s.Contains(GooLyricUri)) LyricBrowser.Navigate(new Uri(s));
//                });


            Observable.FromEvent<NavigationEventArgs>(h => LyricBrowser.Navigated += h, h => LyricBrowser.Navigated -= h)
                .Where(e => e.EventArgs.Uri.AbsoluteUri.Contains(GooLyricUri))
                .Delay(TimeSpan.FromSeconds(3))
                .Take(1)
                .Repeat()
                .ObserveOnDispatcher()
                .Select(e => (e.Sender as WebBrowser).SaveToString())
                .Select(s => Regex.Match(s, @"s.setAttribute\('src', '(.+?)'"))
                .Where(m => m.Success)
                .Select(m => WebRequest.Create(GooUri + m.Groups[1].Value).DownloadStringAsync())
                .Switch()
                .Select(s => Regex.Replace(s.Trim(), @"^draw\(|\);$", ""))
                .Where(s => !string.IsNullOrEmpty(s))
                .ObserveOnDispatcher()
                .Subscribe(jsonArray =>
                {
                    LyricBrowser.InvokeScript("eval", @"
                        var array = " + jsonArray + @";
                        var sb = [];
                        for(var i = 0; i < array.length; i++) sb.push(array[i]);
                        document.getElementById('lyric_area').innerHTML = sb.join('<br />')");
                });
        }
    }
}
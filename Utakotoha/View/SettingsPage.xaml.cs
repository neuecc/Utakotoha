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

namespace Utakotoha.Model.View
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        List<IDisposable> disposables = new List<IDisposable>();

        protected override void OnNavigatedTo(NavigationEventArgs navArgs)
        {
            base.OnNavigatedTo(navArgs);

            var defsetting = Settings.Default;
            AutoSearchCheckBox.IsChecked = defsetting.IsAutoSearchWhenMusicChanged;
            AutoSelectCheckBox.IsChecked = defsetting.IsAutoSelectFirstMatch;
            BgColorCheckBox.IsChecked = defsetting.IsChangeBrowserColor;
            AuthorizedStatus.Text = defsetting.TwitterCredential != null
                ? "Authorized - " + defsetting.TwitterCredential.ScreenName
                : "Not Authorized";

            Observable.Merge(
                    AutoSearchCheckBox.CheckedAsObservable(),
                    AutoSearchCheckBox.UncheckedAsObservable(),
                    AutoSelectCheckBox.CheckedAsObservable(),
                    AutoSelectCheckBox.UncheckedAsObservable(),
                    BgColorCheckBox.CheckedAsObservable(),
                    BgColorCheckBox.UncheckedAsObservable())
                .Subscribe(_ =>
                {
                    var settings = Settings.Default;
                    settings.IsAutoSearchWhenMusicChanged = AutoSearchCheckBox.IsChecked.Value;
                    settings.IsAutoSelectFirstMatch = AutoSelectCheckBox.IsChecked.Value;
                    settings.IsChangeBrowserColor = BgColorCheckBox.IsChecked.Value;
                    settings.Save();
                })
                .Tap(disposables.Add);

            AuthroizeButton.ClickAsObservable()
                .Do(_ =>
                {
                    AuthorizedStatus.Text = "Not Authorized";
                    Settings.Default.TwitterCredential = null;
                    Settings.Default.Save();
                })
                .Subscribe(_ => NavigationService.Navigate(new Uri("/View/TwitterAuth.xaml", UriKind.Relative)))
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
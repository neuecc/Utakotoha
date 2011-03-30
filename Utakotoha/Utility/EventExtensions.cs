using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;

namespace Utakotoha
{
    public static class EventExtensions
    {
        public static IObservable<IEvent<RoutedEventArgs>> LoadedAsObservable(this FrameworkElement target)
        {
            return Observable.FromEvent<RoutedEventHandler, RoutedEventArgs>(
                h => h.Invoke, h => target.Loaded += h, h => target.Loaded -= h);
        }

        public static IObservable<IEvent<System.Windows.Navigation.NavigationEventArgs>> NavigatedAsObservable(this WebBrowser target)
        {
            return Observable.FromEvent<System.Windows.Navigation.NavigationEventArgs>(
                h => target.Navigated += h, h => target.Navigated -= h);
        }
        public static IObservable<IEvent<RoutedEventArgs>> ClickAsObservable(this ButtonBase target)
        {
            return Observable.FromEvent<RoutedEventHandler, RoutedEventArgs>(
                h => h.Invoke, h => target.Click += h, h => target.Click -= h);
        }

        public static IObservable<IEvent<EventArgs>> ClickAsObservable(this IApplicationBarMenuItem target)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => h.Invoke, h => target.Click += h, h => target.Click -= h);
        }
    }
}

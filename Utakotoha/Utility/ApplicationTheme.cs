
using System.Windows;

namespace Utakotoha
{
    public enum ApplicationTheme
    {
        Dark, Light
    }

    public static class ApplicationThemeManager
    {
        public static ApplicationTheme Current
        {
            get
            {
                var lightVisibility = (Visibility)Application.Current.Resources["PhoneLightThemeVisibility"];
                return (lightVisibility == Visibility.Visible)
                    ? ApplicationTheme.Light
                    : ApplicationTheme.Dark;
            }
        }
    }
}
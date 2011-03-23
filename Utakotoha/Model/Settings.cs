﻿using System.IO.IsolatedStorage;
using Codeplex.OAuth;

namespace Utakotoha
{
    // TODO:no tested yet.

    public class Settings
    {
        const string SettingsKey = "__Settings__";

        public bool IsAutoSearchWhenMusicChanged { get; set; }
        public bool IsAutoClickWhenSearchResultIsOne { get; set; }
        public TwitterCredential TwitterCredential { get; set; }

        public Settings()
        {
            // default
            IsAutoSearchWhenMusicChanged = true;
            IsAutoClickWhenSearchResultIsOne = false;
        }

        public void Save()
        {
            IsolatedStorageSettings.ApplicationSettings[SettingsKey] = this;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        public static Settings Load()
        {
            Settings settings;
            return IsolatedStorageSettings.ApplicationSettings.TryGetValue<Settings>(SettingsKey, out settings)
                ? settings
                : new Settings();
        }
    }
}
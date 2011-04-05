using System.IO.IsolatedStorage;
using Codeplex.OAuth;
using System.Runtime.Serialization;

namespace Utakotoha.Model
{
    public class Settings
    {
        public static readonly Settings Default = Settings.Load();

        const string SettingsKey = "__Settings__";

        public TwitterCredential TwitterCredential { get; set; }
        public bool IsAutoSearchWhenMusicChanged { get; set; }
        public bool IsAutoSelectFirstMatch { get; set; }

        public Settings()
        {
            IsAutoSearchWhenMusicChanged = true;
            IsAutoSelectFirstMatch = true;
        }

        public void Save()
        {
            IsolatedStorageSettings.ApplicationSettings[SettingsKey] = new SaveContainer
            {
                IsAutoSearchWhenMusicChanged = this.IsAutoSearchWhenMusicChanged,
                IsAutoSelectFirstMatch = this.IsAutoSelectFirstMatch,
                TwitterKey = (this.TwitterCredential != null) ? TwitterCredential.AccessToken.Key : null,
                TwitterSecret = (this.TwitterCredential != null) ? TwitterCredential.AccessToken.Secret : null,
                TwitterScreenName = (this.TwitterCredential != null) ? TwitterCredential.ScreenName : null
            };
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        public static Settings Load()
        {
            SaveContainer container;
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<SaveContainer>(SettingsKey, out container))
            {
                return new Settings
                {
                    IsAutoSearchWhenMusicChanged = container.IsAutoSearchWhenMusicChanged,
                    IsAutoSelectFirstMatch = container.IsAutoSelectFirstMatch,
                    TwitterCredential = container.TwitterScreenName == null
                        ? null
                        : new TwitterCredential(
                            container.TwitterScreenName,
                            new AccessToken(container.TwitterKey, container.TwitterSecret))
                };
            }
            else
            {
                return new Settings();
            }
        }
    }

    [DataContract]
    public class SaveContainer
    {
        [DataMember]
        public bool IsAutoSearchWhenMusicChanged { get; set; }
        [DataMember]
        public bool IsAutoSelectFirstMatch { get; set; }
        [DataMember]
        public string TwitterScreenName { get; set; }
        [DataMember]
        public string TwitterKey { get; set; }
        [DataMember]
        public string TwitterSecret { get; set; }
    }
}
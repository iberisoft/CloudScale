using System.ComponentModel;
using Xamarin.Essentials;

namespace CloudScale
{
    class Settings : INotifyPropertyChanged
    {
        protected Settings() { }

        public static Settings Default { get; } = new Settings();

        public string ServerHost
        {
            get => Preferences.Get(nameof(ServerHost), "");
            set
            {
                if (ServerHost != value)
                {
                    Preferences.Set(nameof(ServerHost), value);
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(ServerHost)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

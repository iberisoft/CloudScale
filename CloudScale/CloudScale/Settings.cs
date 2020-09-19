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

        public int ServerPort
        {
            get => Preferences.Get(nameof(ServerPort), 1883);
            set
            {
                if (ServerPort != value)
                {
                    Preferences.Set(nameof(ServerPort), value);
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(ServerPort)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

using CloudScale.Shared;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CloudScaleApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            await OpenHost();
        }

        protected override async void OnDisappearing()
        {
            await CloseHost();
        }

        NetClient m_NetClient;
        readonly ObservableCollection<BaseScale> m_RemoteScales = new ObservableCollection<BaseScale>();

        private async Task OpenHost()
        {
            if (string.IsNullOrWhiteSpace(Settings.Default.ServerHost))
            {
                return;
            }

            m_NetClient = new NetClient("cloud/scale");
            m_NetClient.IsConnectedChanged += NetClient_IsConnectedChanged;
            m_NetClient.MessageReceived += NetClient_MessageReceived;
            await m_NetClient.StartAsync(Settings.Default.ServerHost);
            await m_NetClient.SubscribeAsync("+");

            RemoteScalesView.ItemsSource = m_RemoteScales;
            NotifyHostConnected();
        }

        private async Task CloseHost()
        {
            if (m_NetClient == null)
            {
                return;
            }

            await m_NetClient.StopAsync();
            m_NetClient.IsConnectedChanged -= NetClient_IsConnectedChanged;
            m_NetClient.MessageReceived -= NetClient_MessageReceived;
            m_NetClient.Dispose();
            m_NetClient = null;

            NotifyHostConnected();
        }

        public bool IsHostConnected => m_NetClient?.IsConnected == true;

        private void NetClient_IsConnectedChanged(object sender, EventArgs e)
        {
            NotifyHostConnected();
        }

        private void NotifyHostConnected() => OnPropertyChanged(nameof(IsHostConnected));

        private void NetClient_MessageReceived(object sender, NetClient.Message e)
        {
            var deviceId = e.Topic;
            var remoteScale = m_RemoteScales.FirstOrDefault(scale => scale.DeviceId == deviceId);
            if (remoteScale == null)
            {
                remoteScale = new BaseScale { DeviceId = deviceId };
                Dispatcher.BeginInvokeOnMainThread(() => m_RemoteScales.Add(remoteScale));
            }
            remoteScale.FromJsonString(e.Payload);
        }

        private async void OpenSettings(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage { BindingContext = Settings.Default });
        }
    }
}

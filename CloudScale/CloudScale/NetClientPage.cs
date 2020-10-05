using MqttHelper;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CloudScale
{
    public abstract class NetClientPage : ContentPage
    {
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await OpenHost();
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

            await CloseHost();
        }

        protected NetClient NetClient { get; private set; }

        protected virtual async Task OpenHost()
        {
            if (string.IsNullOrWhiteSpace(Settings.Default.ServerHost))
            {
                return;
            }

            NetClient = new NetClient("cloud");
            NetClient.IsConnectedChanged += NetClient_IsConnectedChanged;
            NetClient.MessageReceived += NetClient_MessageReceived;
            await NetClient.StartAsync(Settings.Default.ServerHost, Settings.Default.ServerPort);

            NotifyHostConnected();
        }

        protected virtual async Task CloseHost()
        {
            if (NetClient == null)
            {
                return;
            }

            await NetClient.StopAsync();
            NetClient.IsConnectedChanged -= NetClient_IsConnectedChanged;
            NetClient.MessageReceived -= NetClient_MessageReceived;
            NetClient.Dispose();
            NetClient = null;

            NotifyHostConnected();
        }

        public bool IsHostConnected => NetClient?.IsConnected == true;

        private void NetClient_IsConnectedChanged(object sender, EventArgs e)
        {
            NotifyHostConnected();
        }

        private void NotifyHostConnected() => OnPropertyChanged(nameof(IsHostConnected));

        private async void NetClient_MessageReceived(object sender, NetMessage e)
        {
            var i = e.Topic.IndexOf('/');
            var j = e.Topic.IndexOf('/', i + 1);
            await Device.InvokeOnMainThreadAsync(() => MessageReceivedHandler(e.Topic.Remove(i), e.Topic.Substring(i + 1, j - i - 1), e.Topic.Substring(j + 1), e.Payload));
        }

        protected abstract void MessageReceivedHandler(string deviceType, string deviceId, string subTopic, string payload);
    }
}

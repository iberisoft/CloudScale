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

            NetClient = new NetClient("cloud/scale");
            NetClient.IsConnectedChanged += NetClient_IsConnectedChanged;
            NetClient.MessageReceived += NetClient_MessageReceived;
            await NetClient.StartAsync(Settings.Default.ServerHost);

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

        private void NetClient_MessageReceived(object sender, NetMessage e)
        {
            var i = e.Topic.IndexOf('/');
            MessageReceivedHandler(e.Topic.Remove(i), e.Topic.Substring(i + 1), e.Payload);
        }

        protected abstract void MessageReceivedHandler(string deviceId, string subTopic, string payload);
    }
}

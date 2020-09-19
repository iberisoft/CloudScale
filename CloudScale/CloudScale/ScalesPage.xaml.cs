using CloudScale.Shared;
using MqttHelper;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CloudScale
{
    public partial class ScalesPage : NetClientPage
    {
        public ScalesPage()
        {
            InitializeComponent();

            RemoteScalesView.ItemsSource = m_RemoteScales;
        }

        readonly ObservableCollection<RemoteScale> m_RemoteScales = new ObservableCollection<RemoteScale>();

        protected override async Task OpenHost()
        {
            await base.OpenHost();

            if (NetClient != null)
            {
                await NetClient.SubscribeAsync("scale/+/heartbeat");
                await NetClient.SubscribeAsync("scale/+/weight");
                await NetClient.SubscribeAsync("scale/+/global_position");
            }
        }

        protected override async void MessageReceivedHandler(string deviceType, string deviceId, string subTopic, string payload)
        {
            var remoteScale = m_RemoteScales.FirstOrDefault(scale => scale.DeviceId == deviceId);
            switch (subTopic)
            {
                case "heartbeat":
                    if (remoteScale == null)
                    {
                        remoteScale = new RemoteScale { DeviceId = deviceId };
                        await Device.InvokeOnMainThreadAsync(() => m_RemoteScales.Add(remoteScale));
                        await NetClient.PublishAsync($"scale/{remoteScale.DeviceId}/weight/get");
                        await NetClient.PublishAsync($"scale/{remoteScale.DeviceId}/global_position/get");
                    }
                    break;
                case "weight":
                    remoteScale?.WeightFromJson(payload);
                    break;
                case "global_position":
                    remoteScale?.GlobalPositionFromJson(payload);
                    break;
            }
        }
    }
}

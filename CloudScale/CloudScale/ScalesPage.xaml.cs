using CloudScale.Shared;
using MqttHelper;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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
                await NetClient.SubscribeAsync("scale/+/wifi");
                await NetClient.SubscribeAsync("beacon/+/global_position");
            }
        }

        protected override async void MessageReceivedHandler(string deviceType, string deviceId, string subTopic, string payload)
        {
            if (deviceType == "scale")
            {
                var remoteScale = m_RemoteScales.FirstOrDefault(scale => scale.DeviceId == deviceId);
                switch (subTopic)
                {
                    case "heartbeat":
                        if (remoteScale == null)
                        {
                            remoteScale = new RemoteScale { DeviceId = deviceId };
                            m_RemoteScales.Add(remoteScale);
                            await NetClient.PublishAsync($"scale/{remoteScale.DeviceId}/weight/get");
                            await NetClient.PublishAsync($"scale/{remoteScale.DeviceId}/global_position/get");
                        }
                        await ProcessHeartbeat(remoteScale);
                        break;
                    case "weight":
                        remoteScale?.WeightFromJson(payload);
                        break;
                    case "global_position":
                        remoteScale?.GlobalPositionFromJson(payload);
                        if (remoteScale != null)
                        {
                            remoteScale.IsGlobalPositionCoarse = false;
                        }
                        break;
                    case "wifi":
                        await UpdateBeacons(payload);
                        break;
                }
            }
            else
            {
                switch (subTopic)
                {
                    case "global_position":
                        UpdateBeaconPosition(deviceId, payload);
                        break;
                }
            }
        }

        long m_WiFiScanTimestamp;

        private async Task ProcessHeartbeat(RemoteScale remoteScale)
        {
            if (Stopwatch.GetTimestamp() - m_WiFiScanTimestamp > Stopwatch.Frequency * 5)
            {
                await NetClient.PublishAsync($"scale/{remoteScale.DeviceId}/wifi/scan");
                m_WiFiScanTimestamp = Stopwatch.GetTimestamp();
            }

            if (!remoteScale.HasGlobalPosition || remoteScale.IsGlobalPositionCoarse)
            {
                var coarsePosition = Beacon.ComputeGlobalPosition(m_Beacons);
                if (coarsePosition != null)
                {
                    remoteScale.GlobalPosition = coarsePosition;
                    remoteScale.IsGlobalPositionCoarse = true;
                }
            }
        }

        List<Beacon> m_Beacons = new List<Beacon>();

        private async Task UpdateBeacons(string payload)
        {
            var beacons = JArray.Parse(payload).Select(token => new Beacon { DeviceId = (string)token["ssid"], SignalStrength = (int)token["rssi"] }).ToList();
            foreach (var beacon in beacons)
            {
                var oldBeacon = m_Beacons.FirstOrDefault(oldBeacon2 => oldBeacon2.DeviceId == beacon.DeviceId);
                if (oldBeacon != null)
                {
                    beacon.GlobalPosition = oldBeacon.GlobalPosition;
                }
                else
                {
                    await NetClient.PublishAsync($"beacon/{beacon.DeviceId}/global_position/get");
                }
            }
            m_Beacons = beacons;
        }

        private void UpdateBeaconPosition(string deviceId, string payload)
        {
            var beacon = m_Beacons.FirstOrDefault(beacon2 => beacon2.DeviceId == deviceId);
            if (beacon != null)
            {
                beacon.GlobalPosition = JsonExtension.GlobalPositionFromJson(payload);
            }
        }
    }
}

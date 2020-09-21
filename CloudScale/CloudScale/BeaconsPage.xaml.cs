using CloudScale.Shared;
using MqttHelper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CloudScale
{
    public partial class BeaconsPage : NetClientPage
    {
        public BeaconsPage()
        {
            InitializeComponent();
        }

        private RemoteScale RemoteScale => (RemoteScale)BindingContext;

        protected override async Task OpenHost()
        {
            await base.OpenHost();

            if (NetClient != null)
            {
                await NetClient.SubscribeAsync($"scale/{RemoteScale.DeviceId}/heartbeat");
                await NetClient.SubscribeAsync($"scale/{RemoteScale.DeviceId}/global_position");
                await NetClient.PublishAsync($"scale/{RemoteScale.DeviceId}/global_position/get");
                await NetClient.SubscribeAsync($"scale/{RemoteScale.DeviceId}/wifi");
                await NetClient.SubscribeAsync("beacon/+/global_position");
            }
        }

        protected override async void MessageReceivedHandler(string deviceType, string deviceId, string subTopic, string payload)
        {
            if (deviceType == "scale")
            {
                switch (subTopic)
                {
                    case "heartbeat":
                        await ProcessHeartbeat();
                        break;
                    case "global_position":
                        RemoteScale.GlobalPositionFromJson(payload);
                        RemoteScale.IsGlobalPositionCoarse = false;
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

        private async Task ProcessHeartbeat()
        {
            if (Stopwatch.GetTimestamp() - m_WiFiScanTimestamp > Stopwatch.Frequency * 5)
            {
                await NetClient.PublishAsync($"scale/{RemoteScale.DeviceId}/wifi/scan");
                m_WiFiScanTimestamp = Stopwatch.GetTimestamp();
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
            await Device.InvokeOnMainThreadAsync(() => BeaconsView.ItemsSource = m_Beacons.OrderByDescending(beacon2 => beacon2.SignalStrength));
        }

        private void UpdateBeaconPosition(string deviceId, string payload)
        {
            var beacon = m_Beacons.FirstOrDefault(beacon2 => beacon2.DeviceId == deviceId);
            if (beacon != null)
            {
                beacon.GlobalPosition = JsonExtension.GlobalPositionFromJson(payload);
            }
        }

        private async void SetBeaconPosition(object sender, EventArgs e)
        {
            var beacon = (Beacon)((BindableObject)sender).BindingContext;

            if (await this.Confirm("Beacons", "Set position?"))
            {
                if (RemoteScale.HasGlobalPosition)
                {
                    await NetClient.PublishAsync($"beacon/{beacon.DeviceId}/global_position/set", JsonExtension.GlobalPositionToJson(RemoteScale.GlobalPosition));
                    await NetClient.PublishAsync($"beacon/{beacon.DeviceId}/global_position/get");
                }
                else
                {
                    await this.Inform("Beacons", "No GPS signal.");
                }
            }
        }

        private async void ClearBeaconPosition(object sender, EventArgs e)
        {
            var beacon = (Beacon)((BindableObject)sender).BindingContext;
            if (!beacon.HasGlobalPosition)
            {
                return;
            }

            if (await this.Confirm("Beacons", "Clear position?"))
            {
                await NetClient.PublishAsync($"beacon/{beacon.DeviceId}/global_position/clear");
                await NetClient.PublishAsync($"beacon/{beacon.DeviceId}/global_position/get");
            }
        }
    }
}

using CloudScale.Shared;
using MqttHelper;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CloudScale.Service
{
    static class Program
    {
        static NetClient m_NetClient;

        static async Task Main()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            foreach (var deviceId in Settings.Default.BeaconPositions.Keys.OrderBy(deviceId => deviceId))
            {
                var position = Settings.Default.BeaconPositions[deviceId];
                Log.Information("{DeviceId} position is {Latitude}\u00b0 / {Longitude}\u00b0", deviceId, position.Latitude, position.Longitude);
            }

            using (m_NetClient = new NetClient("cloud/beacon"))
            {
                m_NetClient.MessageReceived += NetClient_MessageReceived;
                await m_NetClient.StartAsync(Settings.Default.ServerHost, Settings.Default.ServerPort);
                Log.Information("Connecting to {Host}:{Port}", Settings.Default.ServerHost, Settings.Default.ServerPort);

                await m_NetClient.SubscribeAsync("+/global_position/get");
                await m_NetClient.SubscribeAsync("+/global_position/set");
                await m_NetClient.SubscribeAsync("+/global_position/clear");

                Console.ReadLine();

                await m_NetClient.StopAsync();
            }
        }

        private static async void NetClient_MessageReceived(object sender, NetMessage e)
        {
            var i = e.Topic.IndexOf('/');
            await MessageReceivedHandler(e.Topic.Remove(i), e.Topic.Substring(i + 1), e.Payload);
        }

        private static async Task MessageReceivedHandler(string deviceId, string subTopic, string payload)
        {
            switch (subTopic)
            {
                case "global_position/get":
                    await GetBeaconPosition(deviceId);
                    break;
                case "global_position/set":
                    SetBeaconPosition(deviceId, payload);
                    break;
                case "global_position/clear":
                    ClearBeaconPosition(deviceId);
                    break;
            }
        }

        private static async Task GetBeaconPosition(string deviceId)
        {
            GlobalPosition position;
            lock (Settings.Default.BeaconPositions)
            {
                Settings.Default.BeaconPositions.TryGetValue(deviceId, out position);
            }

            await m_NetClient.PublishAsync($"{deviceId}/global_position", JsonExtension.GlobalPositionToJson(position));
        }

        private static void SetBeaconPosition(string deviceId, string payload)
        {
            var position = JsonExtension.GlobalPositionFromJson(payload);
            if (position == null)
            {
                return;
            }
            lock (Settings.Default.BeaconPositions)
            {
                Settings.Default.BeaconPositions[deviceId] = position;
                Settings.Default.Save();
                Log.Information("Setting {DeviceId} position to {Latitude}\u00b0 / {Longitude}\u00b0", deviceId, position.Latitude, position.Longitude);
            }
        }

        private static void ClearBeaconPosition(string deviceId)
        {
            lock (Settings.Default.BeaconPositions)
            {
                Settings.Default.BeaconPositions.Remove(deviceId);
                Settings.Default.Save();
                Log.Information("Clearing {DeviceId} position", deviceId);
            }
        }
    }
}

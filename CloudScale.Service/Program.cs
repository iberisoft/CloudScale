using CloudScale.Shared;
using MqttHelper;
using System;
using System.Threading.Tasks;

namespace CloudScale.Service
{
    static class Program
    {
        static NetClient m_NetClient;

        static async Task Main()
        {
            using (m_NetClient = new NetClient("cloud/beacon"))
            {
                m_NetClient.MessageReceived += NetClient_MessageReceived;
                await m_NetClient.StartAsync(Settings.Default.ServerHost);

                await m_NetClient.SubscribeAsync("+/global_position/get");
                await m_NetClient.SubscribeAsync("+/global_position/set");

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
                    await SetBeaconPosition(deviceId, payload);
                    break;
            }
        }

        private static async Task GetBeaconPosition(string deviceId)
        {
            if (Settings.Default.BeaconPositions.TryGetValue(deviceId, out GlobalPosition position))
            {
                await m_NetClient.PublishAsync($"{deviceId}/global_position", JsonExtension.GlobalPositionToJson(position));
            }
        }

        private static async Task SetBeaconPosition(string deviceId, string payload)
        {
            var position = JsonExtension.GlobalPositionFromJson(payload);
            Settings.Default.BeaconPositions[deviceId] = position;
            await m_NetClient.PublishAsync($"{deviceId}/global_position", JsonExtension.GlobalPositionToJson(position));

            Settings.Default.Save();
        }
    }
}

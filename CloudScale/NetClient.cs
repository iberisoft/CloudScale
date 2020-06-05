using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Text;
using System.Threading.Tasks;

namespace CloudScale
{
    class NetClient : IDisposable
    {
        IManagedMqttClient m_MqttClient;

        public NetClient(string topicPrefix)
        {
            TopicPrefix = topicPrefix;
            m_MqttClient = new MqttFactory().CreateManagedMqttClient();
            m_MqttClient.UseApplicationMessageReceivedHandler(e => MessageReceivedHandler(e));
        }

        public void Dispose()
        {
            m_MqttClient?.Dispose();
            m_MqttClient = null;
        }

        public string TopicPrefix { get; }

        public async Task StartAsync(string host)
        {
            var options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer(host)
                .Build();
            var options2 = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(options).Build();
            await m_MqttClient.StartAsync(options2);
        }

        public async Task StopAsync()
        {
            await m_MqttClient.StopAsync();
        }

        public async Task SubscribeAsync(string topic)
        {
            topic = GetFullTopic(topic);
            await m_MqttClient.SubscribeAsync(topic);
        }

        public async Task PublishAsync(string topic, string payload)
        {
            topic = GetFullTopic(topic);
            await m_MqttClient.PublishAsync(topic, payload);
        }

        private void MessageReceivedHandler(MqttApplicationMessageReceivedEventArgs e)
        {
            var e2 = new NetMessage();
            e2.Topic = GetSubTopic(e.ApplicationMessage.Topic);
            e2.Payload = e.ApplicationMessage.Payload != null ? Encoding.UTF8.GetString(e.ApplicationMessage.Payload) : null;
            MessageReceived?.Invoke(this, e2);
        }

        public event EventHandler<NetMessage> MessageReceived;

        private string GetFullTopic(string topic) => $"{TopicPrefix}/{topic}";

        private string GetSubTopic(string topic) => topic.StartsWith(TopicPrefix) ? topic.Substring(TopicPrefix.Length).TrimStart('/') : throw new ArgumentException("Wrong topic", nameof(topic));
    }
}

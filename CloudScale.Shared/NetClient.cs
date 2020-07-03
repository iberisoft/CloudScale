using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Text;
using System.Threading.Tasks;

namespace CloudScale.Shared
{
    public class NetClient : IDisposable
    {
        IManagedMqttClient m_MqttClient;

        public NetClient()
        {
            m_MqttClient = new MqttFactory().CreateManagedMqttClient();
            m_MqttClient.UseConnectedHandler(e => IsConnectedChanged?.Invoke(this, EventArgs.Empty));
            m_MqttClient.UseDisconnectedHandler(e => IsConnectedChanged?.Invoke(this, EventArgs.Empty));
            m_MqttClient.UseApplicationMessageReceivedHandler(e => MessageReceivedHandler(e));
        }

        public NetClient(string baseTopic)
            : this()
        {
            BaseTopic = baseTopic;
        }

        public void Dispose()
        {
            m_MqttClient?.Dispose();
            m_MqttClient = null;
        }

        public string BaseTopic { get; }

        public async Task StartAsync(string host, int? port = null)
        {
            var options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer(host, port)
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

        public bool IsConnected => m_MqttClient?.IsConnected == true;

        public event EventHandler IsConnectedChanged;

        public async Task SubscribeAsync(string topic)
        {
            topic = GetFullTopic(topic);
            await m_MqttClient.SubscribeAsync(topic);
        }

        public async Task PublishAsync(string topic)
        {
            topic = GetFullTopic(topic);
            await m_MqttClient.PublishAsync(topic);
        }

        public async Task PublishAsync(string topic, string payload)
        {
            topic = GetFullTopic(topic);
            await m_MqttClient.PublishAsync(topic, payload);
        }

        private void MessageReceivedHandler(MqttApplicationMessageReceivedEventArgs e)
        {
            var e2 = new Message();
            e2.Topic = GetSubTopic(e.ApplicationMessage.Topic);
            e2.Payload = e.ApplicationMessage.Payload != null ? Encoding.UTF8.GetString(e.ApplicationMessage.Payload) : null;
            MessageReceived?.Invoke(this, e2);
        }

        public event EventHandler<Message> MessageReceived;

        private string GetFullTopic(string topic) => string.IsNullOrWhiteSpace(BaseTopic) ? topic : $"{BaseTopic}/{topic}";

        private string GetSubTopic(string topic) => string.IsNullOrWhiteSpace(BaseTopic) ? topic :
            topic.StartsWith(BaseTopic) ? topic.Substring(BaseTopic.Length).TrimStart('/') : throw new ArgumentException("Wrong topic", nameof(topic));

        public class Message
        {
            public string Topic { get; set; }

            public string Payload { get; set; }
        }
    }
}

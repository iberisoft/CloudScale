WiFiClient networkClient;
PubSubClient client(networkClient);

ServerCallback serverCallback;
String deviceTopic;

void setupServer(ServerCallback callback)
{
	client.setServer(serverHost.c_str(), serverPort);
	client.setCallback(_serverCallback);
	serverCallback = callback;
	deviceTopic = topicPrefix + "/" + deviceId;
}

int connectServer()
{
	if (client.connected())
	{
		return 2;
	}
	else
	{
		Serial.print("Connecting to ");
		Serial.print(serverHost);
		Serial.print(":");
		Serial.println(serverPort);

		if (client.connect(deviceName.c_str()))
		{
			Serial.println("Server connected");
			return 1;
		}
		return 0;
	}
}

void pollServer()
{
	client.loop();
}

void _serverCallback(char* topic, uint8_t* data, uint32_t length)
{
	String topic2 = topic;
	topic2 = topic2.substring(deviceTopic.length() + 1);
	String data2;
	for (int i = 0; i < length; ++i)
	{
		data2 += (char)data[i];
	}
	serverCallback(topic2, data2);
}

void subscribeData(String topic)
{
	topic = deviceTopic + "/" + topic;
	client.subscribe(topic.c_str());

	Serial.print("Subscribing to ");
	Serial.println(topic);
}

void publishData(String topic, String data)
{
	topic = deviceTopic + "/" + topic;
	client.publish(topic.c_str(), data.c_str());

	Serial.print(topic);
	Serial.print(": ");
	Serial.println(data);
}

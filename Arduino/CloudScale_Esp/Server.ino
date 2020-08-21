WiFiClient networkClient;
PubSubClient client(networkClient);

void setupServer()
{
	client.setServer(serverHost.c_str(), serverPort);
}

bool connectServer()
{
	if (client.connected())
	{
		return true;
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
			return true;
		}
		return false;
	}
}

String deviceTopic = topicPrefix + "/" + deviceId;

void publishData(String data)
{
	client.publish(deviceTopic.c_str(), data.c_str());

	Serial.print(deviceTopic);
	Serial.print(": ");
	Serial.println(data);
}

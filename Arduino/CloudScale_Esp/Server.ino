WiFiClient networkClient;
PubSubClient client(networkClient);

void setupServer()
{
	client.setServer(serverHost, serverPort);
}

bool connectServer()
{
	bool connected = false;
	if (client.connected())
	{
		connected = true;
	}
	else
	{
		Serial.print("Connecting to ");
		Serial.print(serverHost);
		Serial.print(":");
		Serial.println(serverPort);
		if (client.connect(deviceId))
		{
			connected = true;
			Serial.println("Server connected");
		}
	}
	return connected;
}

String deviceTopic = String(topicPrefix) + "/" + deviceId;

void publishData(String data)
{
	client.publish(deviceTopic.c_str(), data.c_str());
	Serial.print(deviceTopic);
	Serial.print(": ");
	Serial.println(data);
}

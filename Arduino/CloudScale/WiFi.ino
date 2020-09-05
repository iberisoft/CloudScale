WiFiManager wifiManager;

bool saveConfig = false;

void setupWiFi()
{
	loadSettings();

	char serverHost2[100];
	serverHost.toCharArray(serverHost2, sizeof(serverHost2));
	WiFiManagerParameter serverHostParameter("serverHost", "Server Host", serverHost2, sizeof(serverHost2));
	wifiManager.addParameter(&serverHostParameter);

	char serverPort2[5];
	String(serverPort).toCharArray(serverPort2, sizeof(serverPort2));
	WiFiManagerParameter serverPortParameter("serverPort", "Server Port", serverPort2, sizeof(serverPort2));
	wifiManager.addParameter(&serverPortParameter);

	char topicPrefix2[100];
	topicPrefix.toCharArray(topicPrefix2, sizeof(topicPrefix2));
	WiFiManagerParameter topicPrefixParameter("topicPrefix", "Topic Prefix", topicPrefix2, sizeof(topicPrefix2));
	wifiManager.addParameter(&topicPrefixParameter);

	wifiManager.setSaveConfigCallback(saveConfigCallback);
	wifiManager.autoConnect(deviceName.c_str());

	if (saveConfig)
	{
		serverHost = serverHostParameter.getValue();
		int serverPort2 = String(serverPortParameter.getValue()).toInt();
		if (serverPort2 != 0)
		{
			serverPort = serverPort2;
		}
		topicPrefix = topicPrefixParameter.getValue();

		saveSettings();
	}
}

void saveConfigCallback()
{
	saveConfig = true;
}

void resetWiFi()
{
	wifiManager.resetSettings();
	delay(1000);
	wifiManager.reboot();
}

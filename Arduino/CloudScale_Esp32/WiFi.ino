WiFiConfig networkConfig;

void setupWiFi(void (*connectingCallback)())
{
	Serial.print("Connecting to ");
	Serial.print(networkConfig.Ssid);
	WiFi.begin(networkConfig.Ssid, networkConfig.Password);
	delay(500);
	while (WiFi.status() != WL_CONNECTED)
	{
		connectingCallback();
		Serial.print(".");
	}
	Serial.println("");
	Serial.println("Wi-Fi connected");
	Serial.print("IP address: ");
	Serial.println(WiFi.localIP());
}

void readWiFiConfig()
{
	EEPROM.begin(sizeof(networkConfig));
	EEPROM.get(0, networkConfig);
	if (networkConfig.Header != EEPROM_HEADER)
	{
		inputString("Enter SSID", networkConfig.Ssid, sizeof(networkConfig.Ssid));
		inputString("Enter Wi-Fi password", networkConfig.Password, sizeof(networkConfig.Password));
		networkConfig.Header = EEPROM_HEADER;
		EEPROM.put(0, networkConfig);
		EEPROM.commit();
	}
}

void inputString(const char* prompt, char* buf, unsigned int len)
{
	Serial.print(prompt);
	while (true)
	{
		String line = Serial.readStringUntil('\n');
		if (line.length() > 0)
		{
			line.toCharArray(buf, len);
			break;
		}
		else
		{
			Serial.print(".");
		}
	}
	Serial.println();
}

void clearWiFiConfig()
{
	Serial.println("Clearing ROM...");
	EEPROM.put(0, 0);
	EEPROM.commit();
}

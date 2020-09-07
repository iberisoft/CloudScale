const char* settingsFilePath = "/settings.json";

void loadSettings()
{
	if (SPIFFS.begin())
	{
		Serial.println("Filesystem mounted");
	}
	else
	{
		Serial.println("Filesystem failed to mount");
		return;
	}

	if (SPIFFS.exists(settingsFilePath))
	{
		StaticJsonDocument<1024> doc;

		File file = SPIFFS.open(settingsFilePath, "r");
		deserializeJson(doc, file);
		file.close();
		Serial.println("Loading config file");

		serverHost = (const char*)doc["serverHost"];
		serverPort = doc["serverPort"];
		topicPrefix = (const char*)doc["topicPrefix"];
		JsonArray array = doc["calPoints"].as<JsonArray>();
		loadCalibration(array);
	}
	else
	{
		Serial.println("Config file not found");
	}
}

void saveSettings()
{
	StaticJsonDocument<1024> doc;

	doc["serverHost"] = serverHost;
	doc["serverPort"] = serverPort;
	doc["topicPrefix"] = topicPrefix;
	JsonArray array = doc.createNestedArray("calPoints");
	saveCalibration(array);

	File file = SPIFFS.open(settingsFilePath, "w");
	serializeJson(doc, file);
	file.close();
	Serial.println("Saving config file");
}

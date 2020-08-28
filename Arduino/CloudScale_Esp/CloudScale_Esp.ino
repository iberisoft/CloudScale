#if defined(ESP8266)
#include <SoftwareSerial.h> 
#else
#include <HardwareSerial.h> 
#endif
#include <WiFiManager.h>
#include <PubSubClient.h>
#include <ArduinoJson.h>
#if defined(ESP8266)
#include <FS.h>
#else
#include <SPIFFS.h>
#endif
#include <TinyGPS.h> 
#include "DeviceConfig.h"
#include "Settings.h"

void setup()
{
	Serial.begin(9600);
	Serial.println();

	setupGps();
	setupWiFi();
	setupServer();
}

unsigned long publishTime = 0;

void loop()
{
	pollGps();
	if (millis() - publishTime > 1000)
	{
		publishTime = millis();
		if (connectServer())
		{
			publishData();
		}
	}

	if (Serial.available())
	{
		readCommand();
	}
}

void readCommand()
{
	String command = Serial.readStringUntil('\n');
	if (command == "RST")
	{
		resetWiFi();
	}
}

String currentData;

void publishData()
{
	StaticJsonDocument<256> doc;
	doc["resistance"] = readResistor();
	float latitude, longitude;
	if (readGps(latitude, longitude))
	{
		JsonArray globalPosition = doc.createNestedArray("global_position");
		globalPosition[0] = latitude;
		globalPosition[1] = longitude;
	}
	String data;
	serializeJson(doc, data);
	if (currentData != data)
	{
		currentData = data;
		publishData(currentData);
	}
}

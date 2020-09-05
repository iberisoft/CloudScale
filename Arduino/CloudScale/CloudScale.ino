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
#include "Server.h"

void setup()
{
	Serial.begin(9600);
	Serial.println();

	setupGps();
	setupWiFi();
	setupServer(receiveData);
}

unsigned long updateTime = 0;

void loop()
{
	pollGps();

	switch (connectServer())
	{
	case 1:
	case 2:
		pollServer();
		if (millis() - updateTime > deviceIdle)
		{
			updateTime = millis();
			updateWeight();
			updateGps();
		}
		break;
	}

	if (Serial.available())
	{
		readCommand();
	}
}

void readCommand()
{
	String command = Serial.readStringUntil('\n');
	if (command == "ID")
	{
		Serial.print(deviceId);
		Serial.print('\n');
		return;
	}
	if (command == "RST")
	{
		resetWiFi();
		return;
	}
}

float weightValue = 0;

void updateWeight()
{
	float value = readResistor();
	if (weightValue != value)
	{
		StaticJsonDocument<256> doc;
		doc["value"] = value;
		String data;
		serializeJson(doc, data);
		publishData("weight", data);
		weightValue = value;
	}
}

bool hasLatitudeLongitude = false;
float latitudeValue;
float longitudeValue;

void updateGps()
{
	float latitude, longitude;
	if (readGps(latitude, longitude))
	{
		if (!hasLatitudeLongitude || latitudeValue != latitude || longitudeValue != longitude)
		{
			StaticJsonDocument<256> doc;
			doc["latitude"] = latitude;
			doc["longitude"] = longitude;
			String data;
			serializeJson(doc, data);
			publishData("global_position", data);
			latitudeValue = latitude;
			longitudeValue = longitude;
		}
		hasLatitudeLongitude = true;
	}
	else
	{
		if (hasLatitudeLongitude)
		{
			StaticJsonDocument<256> doc;
			doc["latitude"] = nullptr;
			doc["longitude"] = nullptr;
			String data;
			serializeJson(doc, data);
			publishData("global_position", data);
		}
		hasLatitudeLongitude = false;
	}
}

void receiveData(String topic, String data)
{
}

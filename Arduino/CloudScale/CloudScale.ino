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
#include <LinkedList.h>
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
		subscribeData("weight/calibration/+");
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

float currentWeight = 0;

void updateWeight()
{
	int r = readResistor();
	float weight = convertToWeight(r);
	if (currentWeight != weight)
	{
		StaticJsonDocument<256> doc;
		if (weight != -1)
		{
			doc["value"] = weight;
		}
		else
		{
			doc["value"] = nullptr;
		}
		String data;
		serializeJson(doc, data);
		publishData("weight", data);
		currentWeight = weight;
	}
}

bool hasLatitudeLongitude = false;
float currentLatitude;
float currentLongitude;

void updateGps()
{
	float latitude, longitude;
	if (readGps(latitude, longitude))
	{
		if (!hasLatitudeLongitude || currentLatitude != latitude || currentLongitude != longitude)
		{
			StaticJsonDocument<256> doc;
			doc["latitude"] = latitude;
			doc["longitude"] = longitude;
			String data;
			serializeJson(doc, data);
			publishData("global_position", data);
			currentLatitude = latitude;
			currentLongitude = longitude;
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
	if (topic == "weight/calibration/clear")
	{
		clearCalibration();
		return;
	}
	if (topic == "weight/calibration/add")
	{
		addCalibration(data);
		return;
	}
	if (topic == "weight/calibration/remove")
	{
		removeCalibration(data);
		return;
	}
	if (topic == "weight/calibration/get")
	{
		getCalibration();
		return;
	}
}

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
		subscribeData("weight/get");
		subscribeData("weight/calibration/+");
		subscribeData("global_position/get");
		subscribeData("wifi/scan");
	case 2:
		pollServer();
		if (millis() - updateTime > deviceIdle)
		{
			updateTime = millis();
			heartbeat();
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

void heartbeat()
{
	StaticJsonDocument<256> doc;
	doc["ms"] = updateTime;
	String data;
	serializeJson(doc, data);
	publishData("heartbeat", data);
}

float currentWeight = -1;

void updateWeight()
{
	int r = readResistor();
	float weight = convertToWeight(r);
	if (currentWeight != weight)
	{
		publishWeight(weight);
		currentWeight = weight;
	}
}

void getWeight()
{
	int r = readResistor();
	float weight = convertToWeight(r);
	publishWeight(weight);
}

void publishWeight(float weight)
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
			publishGps(latitude, longitude);
			currentLatitude = latitude;
			currentLongitude = longitude;
		}
		hasLatitudeLongitude = true;
	}
	else
	{
		if (hasLatitudeLongitude)
		{
			publishGps();
		}
		hasLatitudeLongitude = false;
	}
}

void getGps()
{
	float latitude, longitude;
	if (readGps(latitude, longitude))
	{
		publishGps(latitude, longitude);
	}
	else
	{
		publishGps();
	}
}

void publishGps(float latitude, float longitude)
{
	StaticJsonDocument<256> doc;
	doc["latitude"] = latitude;
	doc["longitude"] = longitude;
	String data;
	serializeJson(doc, data);
	publishData("global_position", data);
}

void publishGps()
{
	StaticJsonDocument<256> doc;
	doc["latitude"] = nullptr;
	doc["longitude"] = nullptr;
	String data;
	serializeJson(doc, data);
	publishData("global_position", data);
}

void receiveData(String topic, String data)
{
	if (topic == "weight/get")
	{
		getWeight();
		return;
	}
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
	if (topic == "global_position/get")
	{
		getGps();
		return;
	}
	if (topic == "wifi/scan")
	{
		scanWiFi();
		return;
	}
}

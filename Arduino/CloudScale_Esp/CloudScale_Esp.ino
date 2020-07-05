#if defined(ESP8266)
#include <SoftwareSerial.h> 
#include <ESP8266WiFi.h>
#else
#include <HardwareSerial.h> 
#include <WiFi.h>
#endif
#include <PubSubClient.h>
#include <EEPROM.h>
#include <TinyGPS.h> 
#include "DeviceConfig.h"
#include "WiFiConfig.h"
#include "ServerConfig.h"

void setup()
{
	Serial.begin(9600);
	Serial.println();

	setupUss();
	setupGps();
	readWiFiConfig();
	setupWiFi(readCommand);
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
	if (command == "CLR")
	{
		clearWiFiConfig();
		ESP.restart();
	}
}

String currentData;

void publishData()
{
	String data =
		"{\"Distance\": " + String(readUss()) + "," +
		"\"Resistance\": " + String(readResistor()) + "," +
		"\"GlobalPosition\": " + readGps() + "}";
	if (currentData != data)
	{
		currentData = data;
		publishData(currentData.c_str());
	}
}

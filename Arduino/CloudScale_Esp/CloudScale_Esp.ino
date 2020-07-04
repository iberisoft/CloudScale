#include <HardwareSerial.h> 
#include <TinyGPS.h> 
#include <WiFi.h>
#include <PubSubClient.h>
#include <EEPROM.h>
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

void loop()
{
	pollGps();
	if (connectServer())
	{
		publishData();
	}
	readCommand();
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
		"\"GlobalPosition\": [" + readGps() + "]}";
	if (currentData != data)
	{
		currentData = data;
		publishData(currentData.c_str());
	}
}

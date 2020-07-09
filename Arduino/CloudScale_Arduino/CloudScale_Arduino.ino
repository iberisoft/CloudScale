#include <SoftwareSerial.h> 
#include <TinyGPS.h> 
#include "DeviceConfig.h"

void setup()
{
	Serial.begin(9600);

	setupUss();
	setupGps();
}

void loop()
{
	pollGps();
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
	if (command == "USD")
	{
		readUss();
		return;
	}
	if (command == "GPS")
	{
		readGps();
		return;
	}
	if (command == "R")
	{
		readResistor();
		return;
	}
}

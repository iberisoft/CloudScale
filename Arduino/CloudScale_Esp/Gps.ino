#if defined(ESP8266)
SoftwareSerial Serial2(gpsRxPin, gpsTxPin);
#endif

void setupGps()
{
	Serial2.begin(9600);
}

TinyGPS gps;
float latitude = -100;
float longitude = -100;
unsigned long gpsTime = 0;

void pollGps()
{
	while (Serial2.available())
	{
		char c = Serial2.read();
		if (gps.encode(c))
		{
			gps.f_get_position(&latitude, &longitude);
			gpsTime = millis();
		}
	}

	unsigned long age = millis() - gpsTime;
	if (age > gpsTimeout || age < 0)
	{
		latitude = -100;
		longitude = -100;
		gpsTime = millis();
	}
}

String readGps()
{
	return latitude != -100 && longitude != -100 ? "[" + String(latitude) + "," + String(longitude) + "]" : "null";
}

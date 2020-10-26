#if defined(ESP8266)
SoftwareSerial Serial2(gpsRxPin, gpsTxPin);
#endif

void setupGps()
{
	Serial2.begin(9600);
}

TinyGPS gps;
float latitude = -100;
float longitude = -200;
uint32_t gpsTime = 0;

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

	if (millis() - gpsTime > gpsTimeout)
	{
		latitude = -100;
		longitude = -200;
	}
}

bool readGps(float& latitude2, float& longitude2)
{
	if (latitude != -100 && longitude != -200)
	{
		latitude2 = latitude;
		longitude2 = longitude;
		return true;
	}
	else
	{
		return false;
	}
}

void setupGps()
{
	Serial2.begin(9600);
}

TinyGPS gps;
float latitude = -1;
float longitude = -1;
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
		latitude = -1;
		longitude = -1;
		gpsTime = millis();
	}
}

String readGps()
{
	return String(latitude) + "," + String(longitude);
}

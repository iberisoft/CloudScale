SoftwareSerial gpsSerial(gpsRxPin, gpsTxPin);

void setupGps()
{
	gpsSerial.begin(9600);
}

TinyGPS gps;
float latitude = -1;
float longitude = -1;
unsigned long gpsTime = 0;

void pollGps()
{
	while (gpsSerial.available())
	{
		char c = gpsSerial.read();
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

void readGps()
{
	Serial.print(latitude);
	Serial.print(',');
	Serial.print(longitude);
	Serial.print('\n');
}

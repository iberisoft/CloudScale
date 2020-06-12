void setupUss()
{
	pinMode(ussTriggerPin, OUTPUT);
	pinMode(ussEchoPin, INPUT);
}

void readUss()
{
	digitalWrite(ussTriggerPin, LOW);
	delayMicroseconds(2);
	digitalWrite(ussTriggerPin, HIGH);
	delayMicroseconds(10);
	digitalWrite(ussTriggerPin, LOW);

	float duration = pulseIn(ussEchoPin, HIGH);
	float distance = duration * 0.0343 / 2;
	Serial.print(distance);
	Serial.print('\n');
}

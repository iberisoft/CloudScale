#include <SoftwareSerial.h> 
#include <TinyGPS.h> 

const char* deviceId = "12345";

const int ussTriggerPin = 9;
const int ussEchoPin = 10;
const int gpsRxPin = 3;
const int gpsTxPin = 4;
const int resistorPin = A0;
const int maxResistance = 1023;

SoftwareSerial gpsSerial(gpsRxPin, gpsTxPin);

void setup()
{
  pinMode(ussTriggerPin, OUTPUT);
  pinMode(ussEchoPin, INPUT);
  Serial.begin(9600);
  gpsSerial.begin(9600);
}

void loop()
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
      pollUss();
      return;
  }
  if (command == "GPS")
  {
      pollGps();
      return;
  }
  if (command == "R")
  {
      pollResistor();
      return;
  }
}

void pollUss()
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

TinyGPS gps;

void pollGps()
{
  float latitude = -1;
  float longitude = -1;
  while (gpsSerial.available())
  {
    char c = gpsSerial.read();
    if (gps.encode(c))
    {
      gps.f_get_position(&latitude, &longitude);
    }
  }
  Serial.print(latitude);
  Serial.print(',');
  Serial.print(longitude);
  Serial.print('\n');
}

void pollResistor()
{
  float resistance = (float)analogRead(resistorPin) / maxResistance;
  Serial.print(resistance);
  Serial.print('\n');
}

const char* deviceId = "00000";

#if defined(ESP8266)
const int ussTriggerPin = D6;
const int ussEchoPin = D7;
const int resistorPin = A0;
const int maxResistance = 1023;
const int gpsRxPin = D1;
const int gpsTxPin = D2;
#else
const int ussTriggerPin = 4;
const int ussEchoPin = 5;
const int resistorPin = 34;
const int maxResistance = 4095;
#endif
const int gpsTimeout = 60000;
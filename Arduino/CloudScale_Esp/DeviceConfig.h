String deviceId = "00000";
String deviceName = "CloudScale_" + deviceId;

#if defined(ESP8266)
const int resistorPin = A0;
const int maxResistance = 1023;
const int gpsRxPin = D1;
const int gpsTxPin = D2;
#else
const int resistorPin = 34;
const int maxResistance = 4095;
#endif
const int gpsTimeout = 60000;

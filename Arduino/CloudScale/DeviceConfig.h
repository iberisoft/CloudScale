String deviceId = "00000";
String deviceName = "CloudScale_" + deviceId;

#if defined(ESP8266)
const int resistorPin = A0;
const int gpsRxPin = D1;
const int gpsTxPin = D2;
#else
const int resistorPin = 34;
#endif
const int maxCalPoints = 30;
const int gpsTimeout = 10000;

const int deviceIdle = 1000;

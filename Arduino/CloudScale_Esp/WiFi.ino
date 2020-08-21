WiFiManager wifiManager;

void setupWiFi()
{
	wifiManager.autoConnect(deviceName.c_str());
}

void resetWiFi()
{
	wifiManager.resetSettings();
	wifiManager.reboot();
}

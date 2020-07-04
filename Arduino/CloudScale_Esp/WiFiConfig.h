struct WiFiConfig
{
	unsigned int Header;
	char Ssid[33];
	char Password[64];
};

const unsigned int EEPROM_HEADER = 0x11223344;

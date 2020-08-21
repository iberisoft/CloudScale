# Cloud Scale

The prototype project for IoT integrates:

* 1 kOhm pontentiometer
* [Ublox NEO-6 GPS](https://www.u-blox.com/sites/default/files/products/documents/NEO-6_DataSheet_(GPS.G6-HW-09005).pdf)

The Arduino board accepts four commands from the host computer:

* `ID` gets the device ID
* `R` gets the potentiometer resistance from 0 (min) to 1 (max)
* `GPS` gets the latitude and longitude separated by comma

The application publishes MQTT messages with topic `cloud/scale/{device-id}` and resistance, global position in JSON payload.
Besides that the application subscribes to MQTT messages sent by other devices thru the same application.

Finally, the user can see as her local device ID, resistance, and global position as these values returned by other devices
connected to the same MQTT broker.

## Connections

### Arduino

Pontentiometer | Arduino
---------------|--------
End            | 5V
Wiper          | A0
End            | GND


GPS | Arduino
----|--------
VCC | 5V
RX  | D4 (TX)
TX  | D3 (RX)
GND | GND

### WEMOS D1 Mini

Pontentiometer | ESP8266
---------------|--------
End            | 3.3V
Wiper          | A0
End            | GND


GPS | ESP8266
----|--------
VCC | 5V
RX  | D2 (GPIO4)
TX  | D1 (GPIO5)
GND | GND

### ESP32

Pontentiometer | ESP32
---------------|--------
End            | 3.3V
Wiper          | GPIO34
End            | GND


GPS | ESP32
----|--------
VCC | 5V
RX  | GPIO17 (TX2)
TX  | GPIO16 (RX2)
GND | GND

## Configuration

File `CloudScale.exe.config` has the `ServerHost` setting to define the MQTT broker address.

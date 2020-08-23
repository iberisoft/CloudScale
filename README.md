# Cloud Scale

The prototype project for IoT integrates:

* 1 kOhm pontentiometer
* [Ublox NEO-6 GPS](https://www.u-blox.com/sites/default/files/products/documents/NEO-6_DataSheet_(GPS.G6-HW-09005).pdf)

## Arduino Version

The Arduino board accepts four commands from the host computer:

* `ID` gets the device ID
* `R` gets the potentiometer resistance from 0 (min) to 1 (max)
* `GPS` gets the latitude and longitude separated by comma

The application publishes MQTT messages with topic `cloud/scale/{device-id}` and resistance, global position in JSON payload.
Besides that the application subscribes to MQTT messages sent by other devices thru the same application.

Finally, the user can see as her local device ID, resistance, and global position as these values returned by other devices
connected to the same MQTT broker.

### Connections

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

### Configuration

File `CloudScale.exe.config` has the `ServerHost` setting to define the MQTT broker address.

## ESP Version

![WEMOS D1 Mini](Images/Breadboard.jpg)

The ESP board does not involve a host computer, it rather publishes MQTT messages with topic `cloud/scale/{device-id}` and
resistance, global position in JSON payload directly to the broker.

The user can see the device ID, resistance, and global position returned by devices connected to an MQTT broker via the Android app.

### Connections

#### WEMOS D1 Mini

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

#### ESP32

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

### Configuration

The board initially creates a temporary wireless network with name `CloudScale_xxxxx`. Connect to this network and open the Wi-Fi Manager by navigating to
web page `192.168.4.1`.

![](Images/WiFi-Manager.png)

Click `Configure WiFi` button on the page. After opening a new page, enter `SSID` (alternatively, one can select it from available wireless networks)
and `Password` of the wireless network being used later to access the Internet.

![](Images/WiFi-Config.png)

Finally, the board reboots and connects to the permanent network selected above.

The configuration procedure will restart after rebooting if the currently selected wireless network gets offline. It can be forced by sending `RST`
command to the board via USB cable in a serial monitor.

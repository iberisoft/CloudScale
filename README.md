# Cloud Scale

The prototype project for IoT integrates:

* [HC-SR04 ultrasonic sensor](https://cdn.sparkfun.com/datasheets/Sensors/Proximity/HCSR04.pdf)
* 1 kOhm pontentiometer
* [Ublox NEO-6 GPS](https://www.u-blox.com/sites/default/files/products/documents/NEO-6_DataSheet_(GPS.G6-HW-09005).pdf)

The Arduino board accepts four commands from the host computer:

* `ID` gets the device ID
* `USD` gets the distance in cm measured by ultrasound
* `R` gets the potentiometer resistance from 0 (min) to 1 (max)
* `GPS` gets the latitude and longitude separated by comma

The application publishes MQTT messages with topic `cloud/scale/{device-id}` and distance, resistance, global position in JSON payload.
Besides that the application subscribes to MQTT messages sent by other devices thru the same application.

Finally, the user can see as her local device ID, distance, resistance, and global position as these values returned by other devices
connected to the same MQTT broker.

## Connections

### Arduino

HC-SR04 | Arduino
--------|--------
VCC     | 5V
TRIG    | D9
ECHO    | D10
GND     | GND

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

### ESP32

HC-SR04 | ESP32
--------|--------
VCC     | 5V
TRIG    | GPIO4
ECHO    | GPIO5
GND     | GND

Pontentiometer | Arduino
---------------|--------
End            | 3.3V
Wiper          | GPIO34
End            | GND


GPS | Arduino
----|--------
VCC | 5V
RX  | GPIO17 (TX2)
TX  | GPIO16 (RX2)
GND | GND

## Configuration

File `CloudScale.exe.config` has the `ServerHost` setting to define the MQTT broker address.

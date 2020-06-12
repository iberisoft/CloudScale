void readResistor()
{
	float resistance = (float)analogRead(resistorPin) / maxResistance;
	Serial.print(resistance);
	Serial.print('\n');
}

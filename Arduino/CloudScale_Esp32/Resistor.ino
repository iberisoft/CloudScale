float readResistor()
{
	float resistance = (float)analogRead(resistorPin) / maxResistance;
	return resistance;
}

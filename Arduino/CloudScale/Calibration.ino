struct CalPoint
{
	int r;
	float w;

	static int compare(CalPoint& op1, CalPoint& op2) { return op1.r < op2.r ? -1 : op1.r > op2.r ? 1 : 0; }
};

LinkedList<CalPoint> calPoints;

void loadCalibration(JsonArray& array)
{
	for (int i = 0; i < array.size(); ++i)
	{
		CalPoint calPoint;
		calPoint.r = array[i]["r"];
		calPoint.w = array[i]["w"];
		calPoints.add(calPoint);
	}
}

void saveCalibration(JsonArray& array)
{
	for (int i = 0; i < calPoints.size(); ++i)
	{
		array[i]["r"] = calPoints[i].r;
		array[i]["w"] = calPoints[i].w;
	}
}

float convertToWeight(int r)
{
    for (int i = 0; i < calPoints.size() - 1; ++i)
    {
        if (r >= calPoints[i].r && r <= calPoints[i + 1].r)
        {
            return calPoints[i].w + (r - calPoints[i].r) * (calPoints[i + 1].w - calPoints[i].w) / (calPoints[i + 1].r - calPoints[i].r);
        }
    }
    return -1;
}

void clearCalibration()
{
	calPoints.clear();

	saveSettings();
	getCalibration();
}

void addCalibration(String data)
{
	StaticJsonDocument<256> doc;
	deserializeJson(doc, data);

	CalPoint calPoint;
	calPoint.r = readResistor();
	calPoint.w = doc["value"];
	int i = 0;
	for ( ; i < calPoints.size(); ++i)
	{
		if (calPoints[i].r == calPoint.r)
		{
			break;
		}
	}
	if (i < calPoints.size())
	{
		calPoints[i] = calPoint;
	}
	else
	{
		if (calPoints.size() == maxCalPoints)
		{
			return;
		}
		calPoints.add(calPoint);
		calPoints.sort(CalPoint::compare);
	}

	saveSettings();
	getCalibration();
}

void removeCalibration(String data)
{
	StaticJsonDocument<256> doc;
	deserializeJson(doc, data);

	int index = doc["index"];
	if (index < 0 || index >= calPoints.size())
	{
		return;
	}
	calPoints.remove(index);

	saveSettings();
	getCalibration();
}

void getCalibration()
{
	StaticJsonDocument<1024> doc;
	JsonArray array = doc.to<JsonArray>();
	saveCalibration(array);
	String data;
	serializeJson(doc, data);
	publishData("weight/calibration", data);
}

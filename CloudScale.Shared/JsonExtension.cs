using Newtonsoft.Json.Linq;

namespace CloudScale.Shared
{
    public static class JsonExtension
    {
        public static void WeightFromJson(this RemoteScale scale, string text)
        {
            var obj = JObject.Parse(text);
            scale.Weight = (float?)obj["value"];
        }

        public static void GlobalPositionFromJson(this RemoteScale scale, string text)
        {
            var obj = JObject.Parse(text);
            var latitude = (float?)obj["latitude"];
            var longitude = (float?)obj["longitude"];
            scale.GlobalPosition = latitude != null && longitude != null ? new GlobalPosition(latitude.Value, longitude.Value) : null;
        }

        public static GlobalPosition GlobalPositionFromJson(string text)
        {
            var obj = JObject.Parse(text);
            var latitude = (float)obj["latitude"];
            var longitude = (float)obj["longitude"];
            return new GlobalPosition(latitude, longitude);
        }

        public static string GlobalPositionToJson(GlobalPosition position)
        {
            var obj = new JObject();
            obj["latitude"] = position.Latitude;
            obj["longitude"] = position.Longitude;
            return obj.ToString();
        }
    }
}

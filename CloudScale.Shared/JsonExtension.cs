using Newtonsoft.Json.Linq;

namespace CloudScale.Shared
{
    public static class JsonExtension
    {
        public static void WeightFromJson(this BaseScale scale, string text)
        {
            var obj = JObject.Parse(text);
            scale.Weight = (float)obj["value"];
        }

        public static void GlobalPositionFromJson(this BaseScale scale, string text)
        {
            var obj = JObject.Parse(text);
            var latitude = (float?)obj["latitude"];
            var longitude = (float?)obj["longitude"];
            scale.GlobalPosition = latitude != null && longitude != null ? new GlobalPosition(latitude.Value, longitude.Value) : null;
        }
    }
}

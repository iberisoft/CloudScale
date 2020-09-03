namespace CloudScale.Shared
{
    public class GlobalPosition
    {
        public GlobalPosition(float latitude, float longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public float Latitude { get; }

        public float Longitude { get; }
    }
}

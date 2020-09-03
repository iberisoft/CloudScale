using System.ComponentModel;

namespace CloudScale.Shared
{
    public class BaseScale : INotifyPropertyChanged
    {
        string m_DeviceId;

        public string DeviceId
        {
            get => m_DeviceId;
            set
            {
                if (m_DeviceId != value)
                {
                    m_DeviceId = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeviceId)));
                }
            }
        }

        float m_Weight;

        public float Weight
        {
            get => m_Weight;
            set
            {
                if (m_Weight != value)
                {
                    m_Weight = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Weight)));
                }
            }
        }

        GlobalPosition m_GlobalPosition;

        public GlobalPosition GlobalPosition
        {
            get => m_GlobalPosition;
            set
            {
                if (m_GlobalPosition?.Latitude != value?.Latitude || m_GlobalPosition?.Longitude != value?.Longitude)
                {
                    m_GlobalPosition = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GlobalPosition)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasGlobalPosition)));
                }
            }
        }

        public bool HasGlobalPosition => GlobalPosition != null;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

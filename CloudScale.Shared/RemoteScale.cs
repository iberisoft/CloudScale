using System.ComponentModel;

namespace CloudScale.Shared
{
    public class RemoteScale : INotifyPropertyChanged
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

        float? m_Weight;

        public float? Weight
        {
            get => m_Weight;
            set
            {
                if (m_Weight != value)
                {
                    m_Weight = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Weight)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasWeight)));
                }
            }
        }

        public bool HasWeight => Weight != null;

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

        bool m_IsGlobalPositionCoarse;

        public bool IsGlobalPositionCoarse
        {
            get => m_IsGlobalPositionCoarse;
            set
            {
                if (m_IsGlobalPositionCoarse != value)
                {
                    m_IsGlobalPositionCoarse = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsGlobalPositionCoarse)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsGlobalPositionFine)));
                }
            }
        }

        public bool IsGlobalPositionFine => !IsGlobalPositionCoarse;

        public long AliveTimestamp { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

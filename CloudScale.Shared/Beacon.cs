using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CloudScale.Shared
{
    public class Beacon : INotifyPropertyChanged
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

        int m_SignalStrength;

        public int SignalStrength
        {
            get => m_SignalStrength;
            set
            {
                if (m_SignalStrength != value)
                {
                    m_SignalStrength = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SignalStrength)));
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

        public static GlobalPosition ComputeGlobalPosition(IEnumerable<Beacon> beacons)
        {
            beacons = beacons.Where(beacon => beacon.HasGlobalPosition);
            return beacons.OrderByDescending(beacon => beacon.SignalStrength).FirstOrDefault()?.GlobalPosition;
        }
    }
}

﻿using Newtonsoft.Json.Linq;
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
                    IsPropertyChanged = true;
                }
            }
        }

        float m_Resistance;

        public float Resistance
        {
            get => m_Resistance;
            set
            {
                if (m_Resistance != value)
                {
                    m_Resistance = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Resistance)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResistanceInPercent)));
                    IsPropertyChanged = true;
                }
            }
        }

        public float ResistanceInPercent => Resistance * 100;

        float[] m_GlobalPosition;

        public float[] GlobalPosition
        {
            get => m_GlobalPosition;
            set
            {
                if (m_GlobalPosition?[0] != value?[0] || m_GlobalPosition?[1] != value?[1])
                {
                    m_GlobalPosition = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GlobalPosition)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasGlobalPosition)));
                    IsPropertyChanged = true;
                }
            }
        }

        public bool HasGlobalPosition => GlobalPosition != null;

        protected bool IsPropertyChanged { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ToJsonString()
        {
            var obj = new JObject();
            obj["resistance"] = Resistance;
            if (GlobalPosition != null)
            {
                obj["global_position"] = new JArray(GlobalPosition);
            }
            return obj.ToString();
        }

        public void FromJsonString(string text)
        {
            var obj = JObject.Parse(text);
            Resistance = (float)obj["resistance"];
            if (obj["global_position"] != null)
            {
                GlobalPosition = obj["global_position"].ToObject<float[]>();
            }
        }
    }
}

using CloudScale.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;

namespace CloudScale
{
    class LocalScale : BaseScale
    {
        SerialPort m_Port;
        bool m_IsPolling;

        public static List<string> PortNames
        {
            get
            {
                var portNames = SerialPort.GetPortNames().ToList();
                portNames.Insert(0, "");
                return portNames;
            }
        }

        public LocalScale(string portName)
        {
            if (portName != "")
            {
                m_Port = new SerialPort(portName);
                m_Port.ReadTimeout = 1000;
            }
        }

        public void Start()
        {
            if (m_Port != null && !m_IsPolling)
            {
                m_IsPolling = true;
                Task.Run(PollPort);
            }
        }

        public void Stop()
        {
            m_IsPolling = false;
        }

        private void PollPort()
        {
            while (m_IsPolling)
            {
                try
                {
                    if (!m_Port.IsOpen)
                    {
                        m_Port.Open();
                    }

                    IsPropertyChanged = false;
                    if (DeviceId == null)
                    {
                        DeviceId = GetStringValue("ID");
                    }
                    Distance = GetSingleValue("USD");
                    Resistance = GetSingleValue("R");
                    GlobalPosition = GetSingleValues("GPS");
                    if (IsPropertyChanged)
                    {
                        PropertiesChanged?.Invoke(this, EventArgs.Empty);
                    }

                    Task.Delay(1000).Wait();
                }
                catch (TimeoutException)
                {
                }
                catch (Exception ex)
                {
                    if (m_Port.IsOpen)
                    {
                        m_Port.Close();
                    }

                    UnhandledException?.Invoke(this, ex);

                    Task.Delay(5000).Wait();
                }
            }

            m_Port.Dispose();
        }

        private string GetStringValue(string command)
        {
            m_Port.DiscardInBuffer();
            m_Port.WriteLine(command);
            return m_Port.ReadLine();
        }

        private float GetSingleValue(string command) => float.Parse(GetStringValue(command), CultureInfo.InvariantCulture);

        private float[] GetSingleValues(string command) => GetStringValue(command).Split(',').Select(token => float.Parse(token, CultureInfo.InvariantCulture)).ToArray();

        public event EventHandler PropertiesChanged;

        public event EventHandler<Exception> UnhandledException;
    }
}

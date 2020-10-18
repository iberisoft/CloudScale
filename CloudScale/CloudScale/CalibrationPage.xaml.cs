using CloudScale.Shared;
using MqttHelper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CloudScale
{
    public partial class CalibrationPage : NetClientPage
    {
        public CalibrationPage()
        {
            InitializeComponent();
        }

        private RemoteScale RemoteScale => (RemoteScale)BindingContext;

        protected override async Task OpenHost()
        {
            await base.OpenHost();

            if (NetClient != null)
            {
                await NetClient.SubscribeAsync($"scale/{RemoteScale.DeviceId}/weight");
                await NetClient.PublishAsync($"scale/{RemoteScale.DeviceId}/weight/get");
                await NetClient.SubscribeAsync($"scale/{RemoteScale.DeviceId}/weight/calibration");
                await NetClient.PublishAsync($"scale/{RemoteScale.DeviceId}/weight/calibration/get");
            }
        }

        protected override void MessageReceivedHandler(string deviceType, string deviceId, string subTopic, string payload)
        {
            switch (subTopic)
            {
                case "weight":
                    RemoteScale.WeightFromJson(payload);
                    break;
                case "weight/calibration":
                    UpdateTable(payload);
                    break;
            }
        }

        List<CalPoint> m_Table;

        private void UpdateTable(string payload)
        {
            m_Table = JArray.Parse(payload).Select(token => new CalPoint { Resistance = (int)token["r"], Weight = (float)token["w"] }).ToList();
            TableView.ItemsSource = m_Table;
        }

        private async void AddPoint(object sender, EventArgs e)
        {
            if (float.TryParse(WeightEntry.Text, out float value))
            {
                var obj = new JObject
                {
                    ["value"] = value
                };
                await NetClient.PublishAsync($"scale/{RemoteScale.DeviceId}/weight/calibration/add", obj.ToString());
                await NetClient.PublishAsync($"scale/{RemoteScale.DeviceId}/weight/calibration/get");

                WeightEntry.Text = "";
                WeightEntry.Focus();
            }
        }

        private async void ClearPoints(object sender, EventArgs e)
        {
            if (await this.Confirm("Calibration", "Clear values?"))
            {
                await NetClient.PublishAsync($"scale/{RemoteScale.DeviceId}/weight/calibration/clear");
                await NetClient.PublishAsync($"scale/{RemoteScale.DeviceId}/weight/calibration/get");
            }
        }

        private async void RemovePoint(object sender, EventArgs e)
        {
            if (await this.Confirm("Calibration", "Remove value?"))
            {
                var point = (CalPoint)((BindableObject)sender).BindingContext;
                var obj = new JObject
                {
                    ["index"] = m_Table.IndexOf(point)
                };
                await NetClient.PublishAsync($"scale/{RemoteScale.DeviceId}/weight/calibration/remove", obj.ToString());
                await NetClient.PublishAsync($"scale/{RemoteScale.DeviceId}/weight/calibration/get");
            }
        }

        public class CalPoint
        {
            public int Resistance { get; set; }

            public float Weight { get; set; }
        }
    }
}

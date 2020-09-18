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
                await NetClient.SubscribeAsync($"{RemoteScale.DeviceId}/weight");
                await NetClient.PublishAsync($"{RemoteScale.DeviceId}/weight/get");
                await NetClient.SubscribeAsync($"{RemoteScale.DeviceId}/weight/calibration");
                await NetClient.PublishAsync($"{RemoteScale.DeviceId}/weight/calibration/get");
            }
        }

        List<CalPoint> m_Table;

        protected override async void MessageReceivedHandler(string deviceId, string subTopic, string payload)
        {
            switch (subTopic)
            {
                case "weight":
                    RemoteScale.WeightFromJson(payload);
                    break;
                case "weight/calibration":
                    m_Table = JArray.Parse(payload).Select(token => new CalPoint { Resistance = (int)token["r"], Weight = (float)token["w"] }).ToList();
                    await Device.InvokeOnMainThreadAsync(() => TableView.ItemsSource = m_Table);
                    break;
            }
        }

        private async void AddPoint(object sender, EventArgs e)
        {
            if (float.TryParse(WeightEntry.Text, out float value))
            {
                var obj = new JObject();
                obj["value"] = value;
                await NetClient.PublishAsync($"{RemoteScale.DeviceId}/weight/calibration/add", obj.ToString());

                WeightEntry.Text = "";
                WeightEntry.Focus();
            }
        }

        private async void ClearPoints(object sender, EventArgs e)
        {
            if (await DisplayAlert("Calibration", "Clear values?", "Yes", "No"))
            {
                await NetClient.PublishAsync($"{RemoteScale.DeviceId}/weight/calibration/clear");
            }
        }

        private async void RemovePoint(object sender, EventArgs e)
        {
            if (await DisplayAlert("Calibration", "Remove value?", "Yes", "No"))
            {
                var point = (CalPoint)((BindableObject)sender).BindingContext;
                var obj = new JObject();
                obj["index"] = m_Table.IndexOf(point);
                await NetClient.PublishAsync($"{RemoteScale.DeviceId}/weight/calibration/remove", obj.ToString());

            }
        }

        public class CalPoint
        {
            public int Resistance { get; set; }

            public float Weight { get; set; }
        }
    }
}

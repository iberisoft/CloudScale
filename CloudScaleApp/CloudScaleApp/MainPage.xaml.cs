﻿using CloudScale.Shared;
using MqttHelper;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CloudScaleApp
{
    public partial class MainPage : NetClientPage
    {
        public MainPage()
        {
            InitializeComponent();

            RemoteScalesView.ItemsSource = m_RemoteScales;
        }

        readonly ObservableCollection<BaseScale> m_RemoteScales = new ObservableCollection<BaseScale>();

        protected override async Task OpenHost()
        {
            await base.OpenHost();

            if (NetClient != null)
            {
                await NetClient.SubscribeAsync("+/heartbeat");
                await NetClient.SubscribeAsync("+/weight");
                await NetClient.SubscribeAsync("+/global_position");
            }
        }

        protected override async void MessageReceivedHandler(string deviceId, string subTopic, string payload)
        {
            var remoteScale = m_RemoteScales.FirstOrDefault(scale => scale.DeviceId == deviceId);
            switch (subTopic)
            {
                case "heartbeat":
                    if (remoteScale == null)
                    {
                        remoteScale = new BaseScale { DeviceId = deviceId };
                        await Device.InvokeOnMainThreadAsync(() => m_RemoteScales.Add(remoteScale));
                        await NetClient.PublishAsync($"{remoteScale.DeviceId}/weight/get");
                        await NetClient.PublishAsync($"{remoteScale.DeviceId}/global_position/get");
                    }
                    break;
                case "weight":
                    remoteScale?.WeightFromJson(payload);
                    break;
                case "global_position":
                    remoteScale?.GlobalPositionFromJson(payload);
                    break;
            }
        }

        private async void OpenSettings(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage { BindingContext = Settings.Default });
        }
    }
}

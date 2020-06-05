using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CloudScale
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpenScale();
            await OpenHost();
        }

        private async void Window_Closed(object sender, EventArgs e)
        {
            CloseScale();
            await CloseHost();

            Properties.Settings.Default.Save();
            Application.Current.Shutdown();
        }

        private void PortNameBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                CloseScale();
                OpenScale();
            }
        }

        LocalScale m_Scale;

        private void OpenScale()
        {
            if (m_Scale != null || Properties.Settings.Default.PortName == "")
            {
                return;
            }

            m_Scale = new LocalScale(Properties.Settings.Default.PortName);
            m_Scale.PropertiesChanged += Scale_PropertiesChanged;
            m_Scale.UnhandledException += Scale_UnhandledException;
            m_Scale.Start();

            ScaleControl.DataContext = m_Scale;
        }

        private void CloseScale()
        {
            if (m_Scale == null)
            {
                return;
            }

            m_Scale.Stop();
            m_Scale.PropertiesChanged -= Scale_PropertiesChanged;
            m_Scale.UnhandledException -= Scale_UnhandledException;
            m_Scale = null;

            ScaleControl.DataContext = null;
        }

        private async void Scale_PropertiesChanged(object sender, EventArgs e)
        {
            await UpdateHost();
        }

        private void Scale_UnhandledException(object sender, Exception e)
        {
            Dispatcher.Invoke(() => MessageBox.Show(e.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error));
        }

        NetClient m_NetClient;
        ObservableCollection<BaseScale> m_RemoteScales = new ObservableCollection<BaseScale>();

        private async Task OpenHost()
        {
            if (Properties.Settings.Default.ServerHost == "")
            {
                return;
            }

            m_NetClient = new NetClient("cloud/scale");
            m_NetClient.MessageReceived += NetClient_MessageReceived;
            await m_NetClient.StartAsync(Properties.Settings.Default.ServerHost);
            await m_NetClient.SubscribeAsync("+");

            RemoteScalesControl.DataContext = m_RemoteScales;
        }

        private async Task CloseHost()
        {
            if (m_NetClient == null)
            {
                return;
            }

            await m_NetClient.StopAsync();
            m_NetClient.MessageReceived -= NetClient_MessageReceived;
            m_NetClient.Dispose();
            m_NetClient = null;
        }

        private async Task UpdateHost()
        {
            if (m_NetClient != null)
            {
                await m_NetClient.PublishAsync($"{m_Scale.DeviceId}", m_Scale.ToJsonString());
            }
        }

        private void NetClient_MessageReceived(object sender, NetMessage e)
        {
            var deviceId = e.Topic;
            var remoteScale = m_RemoteScales.FirstOrDefault(scale => scale.DeviceId == deviceId);
            if (remoteScale == null)
            {
                remoteScale = new BaseScale { DeviceId = deviceId };
                Dispatcher.Invoke(() => m_RemoteScales.Add(remoteScale));
            }
            remoteScale.FromJsonString(e.Payload);
        }
    }
}

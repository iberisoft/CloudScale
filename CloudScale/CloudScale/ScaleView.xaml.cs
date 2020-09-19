using System;
using Xamarin.Forms;

namespace CloudScale
{
    public partial class ScaleView : ContentView
    {
        public ScaleView()
        {
            InitializeComponent();
        }

        private async void OpenCalibration(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CalibrationPage { BindingContext = BindingContext });
        }

        private async void OpenBeacons(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BeaconsPage { BindingContext = BindingContext });
        }
    }
}

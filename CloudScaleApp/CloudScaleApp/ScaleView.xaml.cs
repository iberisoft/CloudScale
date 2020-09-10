using System;
using Xamarin.Forms;

namespace CloudScaleApp
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
    }
}

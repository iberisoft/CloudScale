using Xamarin.Forms;

namespace CloudScaleApp
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (ServerHostEntry.Text == "")
            {
                ServerHostEntry.Focus();
            }
        }
    }
}

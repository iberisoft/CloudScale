using Xamarin.Forms;

namespace CloudScaleApp
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            BindingContext = Settings.Default;
        }
    }
}

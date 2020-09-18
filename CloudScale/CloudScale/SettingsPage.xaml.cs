using Xamarin.Forms;

namespace CloudScale
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

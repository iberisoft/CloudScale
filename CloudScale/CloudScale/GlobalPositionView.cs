using CloudScale.Shared;
using Xamarin.Forms;

namespace CloudScale
{
    public class GlobalPositionView : ContentView
    {
        public static readonly BindableProperty GlobalPositionProperty = BindableProperty.Create(nameof(GlobalPosition), typeof(GlobalPosition), typeof(GlobalPositionView));

        public GlobalPosition GlobalPosition
        {
            get => (GlobalPosition)GetValue(GlobalPositionProperty);
            set => SetValue(GlobalPositionProperty, value);
        }
    }
}

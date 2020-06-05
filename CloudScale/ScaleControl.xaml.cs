using System.Windows;
using System.Windows.Controls;

namespace CloudScale
{
    /// <summary>
    /// Interaction logic for ScaleControl.xaml
    /// </summary>
    public partial class ScaleControl : UserControl
    {
        public ScaleControl()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            IsEnabled = DataContext != null;
        }
    }
}

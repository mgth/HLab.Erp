using System.Windows;
using System.Windows.Controls;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Data.Wpf
{
    /// <summary>
    /// Logique d'interaction pour DatabaseConfig.xaml
    /// </summary>
    public partial class DatabaseConfigView : UserControl, IView<ConnectionDataViewModel>
    {

        public DatabaseConfigView()
        {
            InitializeComponent();
        }

        void CbServer_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.DialogResult = true;
            window?.Close();

        }

        void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.DialogResult = false;
            window?.Close();

        }
    }
}

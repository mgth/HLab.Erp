using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl.Wcf;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Wpf;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace HLab.Erp.Core.Wpf.ApplicationServices
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UserControl
        , IView<ViewModeDefault, MainWpfViewModel>
        , IView<ViewModeKiosk, MainWpfViewModel>
    {

        public MainWindow()
        {
            InitializeComponent();

            LoadLayout();

            this.Unloaded += MainWindow_Unloaded;
            this.DataContextChanged += MainWindow_DataContextChanged;
        }

        [Import]
        private IOptionsService _options;

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            SaveLayout();
        }

        private void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var ctx = this.GetValue(ViewLocator.MvvmContextProperty);
        }

        private void SaveLayout()
        {
            var fileName = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), @"CHMP\Echantillonage\layout.xml");

            var dir = Path.GetDirectoryName(fileName);
            if (dir == null) return;

            Directory.CreateDirectory(dir);

            var layoutSerializer = new XmlLayoutSerializer(DockingManager);
            using (var writer = new StreamWriter(fileName))
            {
                layoutSerializer.Serialize(writer);
            }
        }

        private void LoadLayout()
        {
            var layoutSerializer = new XmlLayoutSerializer(DockingManager);

            try
            {
                using (var reader = _options.GetOptionFileReader("layout.xml"))
                {
                    layoutSerializer.Deserialize(reader);
                }
            }
            catch (FileNotFoundException ex)
            {
                var res = MessageBox.Show(ex.Message, "Debug", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (DirectoryNotFoundException ex)
            {
                var res = MessageBox.Show(ex.Message, "Debug", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (InvalidOperationException ex)
            {
                var res = MessageBox.Show(ex.Message, "Debug", MessageBoxButton.OK,
                    MessageBoxImage.Information);                
            }

        }
    }
}

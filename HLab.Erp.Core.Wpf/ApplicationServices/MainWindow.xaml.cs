﻿using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Extentions;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace HLab.Erp.Core.ApplicationServices
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UserControl
        , IView<ViewModeDefault, MainWpfViewModel>
        , IView<ViewModeKiosk, MainWpfViewModel>
    {
        private const string LayoutFileName = "layout.xml";

        public MainWindow()
        {
            InitializeComponent();

            LoadLayout();

            this.Loaded += MainWindow_Loaded;
            this.DataContextChanged += MainWindow_DataContextChanged;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var w = Window.GetWindow(this);
            if (w != null)
            {
                w.Closing += W_Closing;
            }
        }

        private void W_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveLayout();
        }

        [Import]
        private IOptionsService _options;
        [Import]
        private IAclService _acl;

        private void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var ctx = this.GetValue(ViewLocator.MvvmContextProperty);
        }

        private void SaveLayout()
        {
            var layoutSerializer = new XmlLayoutSerializer(DockingManager);
            //using var writer = _options.GetOptionFileWriter(LayoutFileName);
            var sb = new StringBuilder();
            using var writer = new StringWriter(sb);
            layoutSerializer.Serialize(writer);
            _options.SetValue<string>("Layout", sb.ToString(), _acl.Connection.UserId);
        }

        private async void LoadLayout()
        {
            var layoutSerializer = new XmlLayoutSerializer(DockingManager);

            try
            {
                var layout = await _options.GetValue<string>("Layout",_acl.Connection.UserId).ConfigureAwait(true);
                //using var reader = _options.GetOptionFileReader(LayoutFileName);
                if (!string.IsNullOrWhiteSpace(layout))
                {
                    using var reader = new StringReader(layout);
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

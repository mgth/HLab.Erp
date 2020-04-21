﻿using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using AvalonDock.Layout.Serialization;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Acl.Annotations;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using UserControl = System.Windows.Controls.UserControl;

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

        private bool _clicked = false;

        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _clicked = true;
        }

        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            if(!_clicked) return;

            var w = Window.GetWindow(this);
            if (w != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (w.WindowState == WindowState.Maximized)
                    {
                        var width = w.ActualWidth;
                        var height = w.ActualHeight;

                        var pos = e.GetPosition(w);
                        var xRatio = pos.X / width;

                        var absPos = PointToScreen(pos);

                        var ct = PresentationSource.FromVisual(w)?.CompositionTarget;

                        if (ct != null)
                        {
                            var m = ct.TransformToDevice;


                            w.Top = (absPos.Y / m.M22) - pos.Y * (w.Height / height);

                            w.Left = (absPos.X / m.M11) - pos.X * (w.Width / width); 

                            w.WindowState = WindowState.Normal;
                        }

                    }

                    _clicked = false;
                    try
                    {
                        w.DragMove();
                    }
                    catch(InvalidOperationException){}
                }
            }
        }

        private void UIElement_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _clicked = false;
        }
    }
}

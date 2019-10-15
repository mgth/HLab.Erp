using System;
using System.Windows;
using System.Windows.Controls;
using HLab.Core;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl.Wcf;
using HLab.Erp.Acl.Wpf.LoginServices;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Wpf;

namespace HLab.Erp.Acl.Wpf.KioskLogin
{
    /// <summary>
    /// Logique d'interaction pour KioskLoginView.xaml
    /// </summary>
    public partial class KioskLoginView : UserControl, IView<ViewModeKiosk, LoginViewModel>
    {
        [Import]
        private readonly IMessageBus _messageBus;
        public KioskLoginView()
        {
            InitializeComponent();
            Loaded += LoginView_Loaded;

        }

        private void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            var win = this.FindVisualParent<Window>();
            if (win != null)
            {
                _messageBus.Subscribe<UserLoggedInMessage>(m =>
                {
                    win.Closed -= LoginView_Closed;
                    win.Close();
                });

                win.Closed += LoginView_Closed;
            }
        }

        private void LoginView_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel l)
            {
                l.SetPassword(PasswordBox.SecurePassword);
            }
        }

    }

}

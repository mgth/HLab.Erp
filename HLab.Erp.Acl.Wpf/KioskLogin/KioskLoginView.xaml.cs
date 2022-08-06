using System;
using System.Windows;
using System.Windows.Controls;
using HLab.Core.Annotations;
using HLab.Erp.Acl.Annotations;
using HLab.Erp.Acl.LoginServices;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Extensions;

namespace HLab.Erp.Acl.KioskLogin
{
    /// <summary>
    /// Logique d'interaction pour KioskLoginView.xaml
    /// </summary>
    public partial class KioskLoginView : UserControl, IView<ViewModeKiosk, LoginViewModel>
    {
        readonly IMessagesService _messageBus;
        public KioskLoginView(IMessagesService messageBus)
        {
            _messageBus = messageBus;
            InitializeComponent();
            Loaded += LoginView_Loaded;

        }

        void LoginView_Loaded(object sender, RoutedEventArgs e)
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

        void LoginView_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel l)
            {
                l.SetPassword(PasswordBox.SecurePassword);
            }
        }

    }

}

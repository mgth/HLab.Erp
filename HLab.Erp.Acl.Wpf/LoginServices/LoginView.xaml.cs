using System;
using System.Windows;
using System.Windows.Controls;
using HLab.Core;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Extentions;

namespace HLab.Erp.Acl.LoginServices
{
    /// <summary>
    /// Logique d'interaction pour Login.xaml
    /// </summary>
    public partial class LoginView : UserControl,
        IView<ViewModeDefault,LoginViewModel>, IViewClassDefault
    {
        
        private readonly IMessageBus _messageBus;
        [Import] public LoginView(IMessageBus messageBus)
        {
            _messageBus = messageBus;
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

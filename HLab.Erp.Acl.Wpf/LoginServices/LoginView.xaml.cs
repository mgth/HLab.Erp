using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using HLab.Core;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Extensions;

namespace HLab.Erp.Acl.LoginServices
{
    /// <summary>
    /// Logique d'interaction pour Login.xaml
    /// </summary>
    public partial class LoginView : UserControl,
        IView<ViewModeDefault,LoginViewModel>, IViewClassDefault
    {
        
        private readonly IMessageBus _messageBus;
        public LoginView(IMessageBus messageBus)
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
                    Application.Current.Dispatcher.InvokeAsync(win.Close) ;
                });

                win.Closed += LoginView_Closed;
            }
        }

        private void LoginView_Closed(object sender, EventArgs e)
        {
            if (DataContext is LoginViewModel l)
            {
                l.CancelCommand.Execute(null);
            }
            Application.Current.Shutdown();
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel l)
            {
                l.Credential = new NetworkCredential(l.Login, PasswordBox.Password);
            }
        }

    }
}

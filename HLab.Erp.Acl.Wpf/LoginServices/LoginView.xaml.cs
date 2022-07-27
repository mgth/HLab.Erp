using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
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
        readonly IMessageBus _messageBus;
        public LoginView(IMessageBus messageBus)
        {
            _messageBus = messageBus;
            InitializeComponent();
            Loaded += LoginView_Loaded;

        }

        void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            var win =  Window.GetWindow(this); //this.FindVisualParent<Window>();
            if (win != null)
            {
                _messageBus.Subscribe<UserLoggedInMessage>(m =>
                {
                    win.Closed -= LoginView_Closed;
                    Application.Current.Dispatcher.InvokeAsync(win.Close) ;
                });

                win.Closed += LoginView_Closed;


                win.Width = 400;
                win.Height = 300;
                win.Left = (SystemParameters.PrimaryScreenWidth - win.Width) / 2;
                win.Top = (SystemParameters.PrimaryScreenHeight - win.Height) / 2;

            }
        }

        void LoginView_Closed(object sender, EventArgs e)
        {
            if (DataContext is LoginViewModel l)
            {
                l.CancelCommand.Execute(null);
            }
            Application.Current.Shutdown();
        }

        void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel l)
            {
                l.Credential = new NetworkCredential(l.Login, PasswordBox.Password);
            }
        }

    }
}

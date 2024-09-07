using System.Net;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using HLab.Core.Annotations;
using HLab.Erp.Acl.LoginServices;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Acl.Avalonia.LoginServices
{
    /// <summary>
    /// Logique d'interaction pour Login.xaml
    /// </summary>
    public partial class LoginView : UserControl,
        IView<DefaultViewMode,LoginViewModel>, IDefaultViewClass
    {
        readonly IMessagesService _messageBus;
        public LoginView(IMessagesService messageBus)
        {
            _messageBus = messageBus;
            InitializeComponent();
            Loaded += LoginView_Loaded;

        }

        void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            //this.FindVisualParent<Window>();
            if (TopLevel.GetTopLevel(this) is not Window win) return;

            _messageBus.Subscribe<UserLoggedInMessage>(m =>
            {
                win.Closed -= LoginView_Closed;
                Dispatcher.UIThread.InvokeAsync(win.Close);
            });

            win.Closed += LoginView_Closed;


            win.Width = 400;
            win.Height = 300;
            //win.Left = (SystemParameters.PrimaryScreenWidth - win.Width) / 2;
            //win.Top = (SystemParameters.PrimaryScreenHeight - win.Height) / 2;
        }

        void LoginView_Closed(object sender, EventArgs e)
        {
            if (DataContext is LoginViewModel l)
            {
                l.CancelCommand.Execute(null);
            }
            // TODO : replace with HLab implementation.
            Dispatcher.UIThread.BeginInvokeShutdown(DispatcherPriority.Normal);
        }

        //void PasswordBox_OnPasswordChanged(object sender, EventArgs e)
        //{
        //    if (DataContext is LoginViewModel l)
        //    {
        //        l.Credential = new (l.Username, PasswordBox.Text);
        //    }
        //}

        void PasswordBox_OnPasswordChanged(object? sender, TextChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Net;
using System.Security;
using System.Windows;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Data.Observables;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl.Wpf.LoginServices
{
    [Export(typeof(LoginViewModel))]
    public class LoginViewModel : ViewModel<LoginViewModel>
    {
        public string Title => "Connexion";
        [Import]
        private readonly IAclService _logon;

        [Import]
        public ILocalizationService LocalizationService { get; }
#if DEBUG
        public Visibility DebugVisibility => Visibility.Visible;
#else
        public Visibility DebugVisibility => Visibility.Collapsed;
#endif

        private IProperty<string> _login = H.Property<string>(c => c
#if DEBUG
                    .Default("admin")
#else
                    .Default("")
#endif
        );
        public string Login
        {
            get => _login.Get();
            set
            {
                if(_login.Set(value))
                {
                    Credential.UserName = value;
                }
            }
        }

        private IProperty<string> _password = H.Property<string>(c => c
#if DEBUG
        .Default("VEqwosdLL6ZPetwK5aFlIg")
#else
        .Default("")

#endif
        );
        public string Password
        {
            get => _password.Get();
            set => _password.Set(value);
        }

        public NetworkCredential Credential
        {
            get => _credential.Get();
            set
            {
                if(_credential.Set(value))
                {
                    Login = value.UserName;
                    Password = _logon.Crypt(value.SecurePassword);
                }
            }
        }
        private readonly IProperty<NetworkCredential> _credential = H.Property<NetworkCredential>(c => c
#if DEBUG
        
        .Default(new NetworkCredential("admin", "blagueur"))
#else
        .Default(new NetworkCredential("", ""))
#endif
        );

        public string Message
        {
            get => _message.Get();
            set => _message.Set(value);
        }
        private readonly IProperty<string> _message = H.Property<string>(nameof(Message));

        public User User
        {
            get => _user.Get();
            set
            {
                if (_user.Set(value))
                {
                    Login = value.Login;
                }
            }
        }
        private IProperty<User> _user = H.Property<User>();

        public string PinView
        {
            get => _pinView.Get();
            set => _pinView.Set(value);
        }
        private IProperty<string> _pinView = H.Property<string>();

        private string _pin = "";
        public ICommand NumPadCommand => _numPadCommand.Get();
        private readonly IProperty<ICommand> _numPadCommand = H.Property<ICommand>(c => c
            .On(e => e.Login)
            .Command(
            (e,n) =>
            {
                if (e._pin.Length > 4) e._pin = "";
                e._pin += (string)n;

                e.PinView = new String('.', e._pin.Length);

                if (e._pin.Length == 4)
                {
                    e.Message = e._logon.Login(new NetworkCredential(e.Credential.UserName, e._pin), true);
                    e._pin = "";
                    e.PinView = "";
                }
            },
            (e) => (e.Login?.Length ?? 0) > 0)
        );

        public ICommand LoginCommand => _loginCommand.Get();
        private IProperty<ICommand> _loginCommand = H.Property<ICommand>(c => c
            .On(e => e.Login)
            .Command(
                (e) =>
                {
                    e.Message = e._logon.Login(e.Credential);
                },
                (e) => (e.Login?.Length??0)>0)                      
        );

        public void SetPassword(SecureString password)
        {
            Credential.SecurePassword = password;
        }

        private readonly IProperty<ObservableQuery<User>> _userList = H.Property<ObservableQuery<User>>(nameof(UserList));

        //TODO : respect template
        [Import(InjectLocation.AfterConstructor)]
        private void _setUserList(ObservableQuery<User> userlist)
        {
            _userList.Set(userlist
                .AddFilter(u => u.Note.Contains("<balances>"))
                .FluentUpdate()            
            );
        }

        public ObservableQuery<User> UserList => _userList.Get();
    }
}

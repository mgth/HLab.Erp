using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security;
using System.Windows;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Data.Observables;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;
using H = HLab.Notify.PropertyChanged.NotifyHelper<HLab.Erp.Acl.LoginServices.LoginViewModel>;

namespace HLab.Erp.Acl.LoginServices
{
    [Export(typeof(LoginViewModel))]
    public class LoginViewModel : INotifyPropertyChanged
    {
        [Import] public LoginViewModel(IAclService logon)
        {
            _logon = logon;
            H.Initialize(this, a => PropertyChanged?.Invoke(this,a));
        }

        public string Title => "Connexion";
        
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
        public ICommand NumPadCommand { get; } = H.Command(c => c
            
            .CanExecute((e) => (e.Login?.Length ?? 0) > 0)
            .Action(
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
            }
            )
            .On(e => e.Login)
            .On(e => e.Password)
            .CheckCanExecute()
        );

        public ICommand LoginCommand { get; } = H.Command( c => c
            .CanExecute(e => (e.Login?.Length??0)>0)
            .Action( e =>
                {
                    e.Message = e._logon.Login(e.Credential);
                })                      
            .On(e => e.Login).CheckCanExecute()
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
                .AddFilter(()=>u => u.Note.Contains("<balances>"))
                .FluentUpdate()            
            );
        }

        public ObservableQuery<User> UserList => _userList.Get();
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

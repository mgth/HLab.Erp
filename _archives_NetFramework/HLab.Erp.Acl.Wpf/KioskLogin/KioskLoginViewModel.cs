using System;
using System.Net;
using System.Security;
using System.Windows;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Mvvm;
using HLab.Notify;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl.Wpf.KioskLogin
{
    [Export(typeof(KioskLoginViewModel))]
    public class KioskLoginViewModel : ViewModel<KioskLoginViewModel>
    {
        [Import]
        private readonly IAclService _logon;
        [Import]
        private readonly IDbService _db;

#if DEBUG
        public Visibility DebugVisibility => Visibility.Visible;
#else
        public Visibility DebugVisibility => Visibility.Collapsed;
#endif

        public string Login
        {
            get => _login.Get();
            set => _login.Set(value, () => Credential.UserName = value);
        }
        private readonly IProperty<string> _login = H.Property<string>(c => c
#if DEBUG
        .Default("admin")
#endif
        );


        public string Password
        {
            get => _password.Get();
            set => _password.Set(value);
        }
        private readonly IProperty<string> _password = H.Property<string>(c => c
#if DEBUG
        .Default("VEqwosdLL6ZPetwK5aFlIg==")
#endif
        );

        public string Pin
        {
            get => _pin.Get();
            set => _pin.Set(value);
        }
        private readonly IProperty<string> _pin = H.Property<string>(c => c
#if DEBUG
        .Default("VEqwosdLL6ZPetwK5aFlIg==")
#endif
        );


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
#endif
        );

        public string Message
        {
            get => _message.Get();
            set => _message.Set(value);
        }
        private readonly IProperty<string> _message = H.Property<string>(c => c );

        public User User
        {
            get => _user.Get();
            set => _user.Set(value);
        }
        private readonly IProperty<User> _user = H.Property<User>(c => c);

        //public ObservableQuery<User> UsersList => _usersList.Get();
        //private readonly IProperty<ObservableQuery<User>> _usersList = H.Property<ObservableQuery<User>>(c => c.
        //    Set(e => new ObservableQuery<User>(e._db).FluentUpdate())
        //);

        [Import]
        public ObservableQuery<User> UsersList
        {
            get => _usersList.Get();
            set => _usersList.Set(value.FluentUpdate());
        }

        private readonly IProperty<ObservableQuery<User>> _usersList = H.Property<ObservableQuery<User>>(c => c
            //.On(e => e)
            //.Update()
        );



        public string PinView
        {
            get => _pinView.Get();
            set => _pinView.Set(value);
        }
        private IProperty<string> _pinView= H.Property<string>(c => c.Default(""));

        private string _pinCode = "";
        public ICommand NumPadCommand => _numPadCommand.Get();
        private IProperty<ICommand> _numPadCommand = H.Property<ICommand>(c => c
            .On(e => e.Login)
            .Command(
            (e,n) =>
            {
                if (e._pinCode.Length > 4) e._pinCode = "";
                e._pinCode += (string)n;

                e.PinView = new String('.', e._pinCode.Length);

                if(e._pinCode.Length==4)
                    e.Message = e._logon.Login(new NetworkCredential(e.Credential.UserName, e._pinCode), true);
            },
                e => e.Login.Length> 0
            )
        );

        public ICommand LoginCommand => _loginCommand.Get();
        private IProperty<ICommand> _loginCommand = H.Property<ICommand>(c => c
        .On(e => e.Login)
        .Command(
            e =>
            {
                e.Message = e._logon.Login(e.Credential);
            },
            e => e.Login.Length > 0)            
        );
        public void SetPassword(SecureString password)
        {
            Credential.SecurePassword = password;
        }
    }
}

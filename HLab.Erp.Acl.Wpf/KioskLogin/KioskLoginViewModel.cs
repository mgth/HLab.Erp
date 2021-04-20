using System;
using System.Net;
using System.Security;
using System.Windows;
using System.Windows.Input;
using Grace.DependencyInjection.Attributes;
using HLab.Erp.Data.Observables;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl.KioskLogin
{
    using H = H<KioskLoginViewModel>;

    [Export(typeof(KioskLoginViewModel))]
    public class KioskLoginViewModel : ViewModel
    {
       
        private readonly IAclService _logon;
        public KioskLoginViewModel(IAclService logon)
        {
            _logon = logon;
            H.Initialize(this);
        }

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


        public ICommand NumPadCommand { get; } = H.Command(c => c
            .CanExecute(e => e.Login.Length > 0)
            .Action(
            async (e,n) =>
            {
                if (e._pinCode.Length > 4) e._pinCode = "";
                e._pinCode += (string)n;

                e.PinView = new String('.', e._pinCode.Length);

                if(e._pinCode.Length==4)
                    e.Message = await e._logon.Login(new NetworkCredential(e.Credential.UserName, e._pinCode), true);
            }
            )
            .On(e => e.Login)
            .CheckCanExecute()
        );

        public ICommand LoginCommand { get; } = H.Command(c => c
            .CanExecute(e => e.Login.Length > 0)
            .Action(
            async e =>
            {
                e.Message = await e._logon.Login(e.Credential);
            }
            ) .On(e => e.Login).CheckCanExecute()
        );
        public void SetPassword(SecureString password)
        {
            Credential.SecurePassword = password;
        }
    }
}

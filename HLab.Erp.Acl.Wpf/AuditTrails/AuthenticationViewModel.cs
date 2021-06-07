using System.Net;
using System.Security;
using System.Windows;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl.AuditTrails
{
    using H = H<AuthenticationViewModel>;

    public class AuthenticationViewModel : ViewModel
    {
        protected IAclService Acl;
        public void Inject(IAclService acl)
        {
            Acl = acl;
            H.Initialize(this);
        }

        public string Message
        {
            get => _message.Get();
            set => _message.Set(value);
        }
        private readonly IProperty<string> _message = H.Property<string>();


        public void SetPassword(SecureString password)
        {
            Credential.SecurePassword = password;
        }

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
        private readonly IProperty<string> _login = H.Property<string>(c => c
#if DEBUG
                .Default("administrateur")
#else
                    .Default("")
#endif
        );

        public User User
        {
            get => _user.Get();
            set
            {
                if (_user.Set(value))
                {
                    Login = value?.Login;
                }
            }
        }
        private readonly IProperty<User> _user = H.Property<User>();


        public string Password
        {
            get => _password.Get();
            set => _password.Set(value);
        }
        private readonly IProperty<string> _password = H.Property<string>(c => c
#if DEBUG
                .Default("VEqwosdLL6ZPetwK5aFlIg")
#else
        .Default("")

#endif
        );

        public NetworkCredential Credential
        {
            get => _credential.Get();
            set
            {
                if(_credential.Set(value))
                {
                    Login = value?.UserName;
                    Password = Acl.Crypt(value.SecurePassword);
                }
            }
        }
        private readonly IProperty<NetworkCredential> _credential = H.Property<NetworkCredential>(c => c
#if DEBUG
        
                .Default(new NetworkCredential("administrateur", "blagueur"))
#else
        .Default(new NetworkCredential("", ""))
#endif
        );



#if DEBUG
        public Visibility DebugVisibility => Visibility.Visible;
#else
        public Visibility DebugVisibility => Visibility.Collapsed;
#endif
        //public ObservableQuery<User> UserList { get; } = H.Query<User>(c => c
        
        //);

        ////TODO : respect template
        //[Import(InjectLocation.AfterConstructor)]
        //private void _setUserList(ObservableQuery<User> userlist)
        //{
        //    _userList.Set(userlist
        //        .AddFilter(()=>u => u.Note.Contains("<balances>"))
        //        .FluentUpdate()            
        //    );
        //}
    }
}
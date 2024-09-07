using System.Net;
using System.Security;
using System.Windows;
using HLab.Mvvm;
using HLab.Mvvm.ReactiveUI;

namespace HLab.Erp.Acl.AuditTrails;

public class AuthenticationViewModel(IAclService acl) : ViewModel
{
    protected readonly IAclService Acl = acl;
    public User User
    {
        get => _user;
        set
        {
            if (SetAndRaise(ref _user, value))
            {
                Username = value?.Username;
            }
        }
    }
    User _user;

    public string Username
    {
        get => _username;
        set
        {
            if(SetAndRaise(ref _username, value))
            {
                Credential.UserName = value;
            }
        }
    }
    string _username = 
#if DEBUG
        "administrateur";
#else
        "";
#endif
    public string Password
    {
        get => _password;
        set => SetAndRaise(ref _password,value);
    }

    string _password = 
#if DEBUG
        "VEqwosdLL6ZPetwK5aFlIg";
#else
        "";
#endif
    public NetworkCredential Credential
    {
        get => _credential;
        set
        {
            if (!SetAndRaise(ref _credential, value)) return;
            if (value != null)
            {
                Username = value.UserName;
                Password = Acl.Crypt(value.SecurePassword);
            }
            else
            {
                Username = "";
                Password = "";
            }
        }
    }

    NetworkCredential _credential = 
#if DEBUG
    new NetworkCredential("administrateur", "blagueur");
#else
    new NetworkCredential("", "");
#endif

    public string Message
    {
        get => _message;
        set => SetAndRaise(ref _message,value);
    }
    string _message = "";


    public void SetPassword(SecureString password)
    {
        Credential.SecurePassword = password;
    }

#if DEBUG
    public bool DebugVisibility => true;
#else
    public bool DebugVisibility => false;
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
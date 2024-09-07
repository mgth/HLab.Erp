using System.Net;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.Erp.Data.Observables;
using HLab.Mvvm.ReactiveUI;
using ReactiveUI;

namespace HLab.Erp.Acl.LoginServices;

public class KioskLoginViewModel : ViewModel
{

#if DEBUG
    public bool DebugVisibility => true;
#else
    public bool DebugVisibility => false;
#endif

    public string Username
    {
        get => _username;
        set {
            if (!SetAndRaise(ref _username, value)) return;
            Credential = new (value, Password);
        }
    }
    string _username = 
#if DEBUG
        "admin";
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
        "VEqwosdLL6ZPetwK5aFlIg==";
#else
        "";
#endif

    public string Pin
    {
        get => _pin;
        set => SetAndRaise(ref _pin,value);
    }

    string _pin = 
#if DEBUG
        "VEqwosdLL6ZPetwK5aFlIg==";
#else
        "";
#endif



    public NetworkCredential Credential
    {
        get => _credential;
        set
        {
            if (!SetAndRaise(ref _credential, value)) return;
            Username = value.UserName;
            Password = _acl.Crypt(value.SecurePassword);
        }
    }

    NetworkCredential _credential = 
#if DEBUG
        new NetworkCredential("admin", "blagueur");
#else
        new NetworkCredential("", "");
#endif

    public string Message
    {
        get => _message;
        set => SetAndRaise(ref _message,value);
    }
    string _message;

    public User User
    {
        get => _user;
        set => SetAndRaise(ref _user,value);
    }
    User _user;

    //public ObservableQuery<User> UsersList => _usersList.Get();
    //private ObservableQuery<User> _usersList = H.Property<ObservableQuery<User>>(c => c.
    //    Set(e => new ObservableQuery<User>(e._db).FluentUpdate())
    //);

    public ObservableQuery<User> UsersList
    {
        get => _usersList;
        set => SetAndRaise(ref _usersList, value.FluentUpdate());
    }

    ObservableQuery<User> _usersList;
    //.On(e => e)
    //.Update()



    public string PinView
    {
        get => _pinView;
        set => SetAndRaise(ref _pinView,value);
    }
    string _pinView = "";

    string _pinCode = "";
    readonly IAclService _acl;

    public KioskLoginViewModel(IAclService acl)
    {
        _acl = acl;

        NumPadCommand =  ReactiveCommand.CreateFromTask<string>(NumPad);

        LoginCommand = ReactiveCommand.CreateFromTask(
            LoginAsync, 
            this.WhenAnyValue(
                e => e.Username, 
                e => e.Password, 
                (userName, password) => userName.Length > 0 && password.Length > 0
            ));

    }


    public ICommand NumPadCommand { get; }

    async Task NumPad(string value)
    {
        if (_pin.Length > 4) _pin = "";
        _pin += value;

        PinView = new string('.', _pin.Length);

        if (_pin.Length != 4) return;
        Message = await _acl.Login(new (Credential.UserName, _pin), true);
        _pin = "";
        PinView = "";
    }


    public ICommand LoginCommand { get; }
    async Task LoginAsync()
    {
        Message = "";
        Message = await Task.Run(() => _acl.Login(Credential));
    }

    public void SetPassword(SecureString password)
    {
        Credential.SecurePassword = password;
    }
}
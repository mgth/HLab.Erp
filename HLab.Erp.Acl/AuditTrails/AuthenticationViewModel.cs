using System;
using System.Net;
using System.Reactive.Linq;
using System.Security;
using HLab.Base.ReactiveUI;
using HLab.Mvvm.ReactiveUI;
using ReactiveUI;

namespace HLab.Erp.Acl.AuditTrails;

public abstract class AuthenticationViewModel : ViewModel
{
    protected readonly IAclService Acl  ;

    protected AuthenticationViewModel(IAclService acl)
    {
        Acl = acl;

        this.WhenAnyValue(e =>  e.User)
            .Select(u => u?.Username)
            .Do(u => Username = u??"")
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe();

        this.WhenAnyValue(e =>  e.Username)
            .Do(u => Credential.UserName = u)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe();

        this.WhenAnyValue(e =>  e.Credential)
            .Do(c =>
            {
                Username = c.UserName;
                Password = Acl.Crypt(c.SecurePassword);
            })
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe();
    }


    public User? User { get; set => this.SetAndRaise(ref field, value); }

    public string Username { get ; set => this.SetAndRaise(ref field, value); } = "";

    public string Password { get ; set => this.SetAndRaise(ref field,value); } = "";

    public NetworkCredential Credential { get ; set => this.SetAndRaise(ref field, value); } = new ("", "");

    public string Message { get; set => this.SetAndRaise(ref field,value); } = "";

    public void SetPassword(SecureString password) { Credential.SecurePassword = password; }

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
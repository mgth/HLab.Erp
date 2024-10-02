using HLab.Erp.Data;
using NPoco;
using ReactiveUI;

namespace HLab.Erp.Acl;

//[Key("Id", AutoIncrement = false)]
//[SoftIncrementAttribut]
public class Connection : Entity
{
    public int? UserId
    { 
        get => _userId;
        set => this.RaiseAndSetIfChanged(ref _userId, value);    
    }
    int? _userId;
    public User? User
    { 
        get => _user.Value;
        set => UserId = value.Id;    
    }
    readonly ObservableAsPropertyHelper<User?> _user;

    public string Account
    {
        get => _account;
        set => this.RaiseAndSetIfChanged(ref _account, value);
    }
    string _account = "";

    public string Domain
    {
        get => _domain;
        set => this.RaiseAndSetIfChanged(ref _domain, value);
    }
    string _domain = "";

    public string Workstation
    {
        get => _workstation;
        set => this.RaiseAndSetIfChanged(ref _workstation, value);
    }
    string _workstation = "";

    public string Os
    {
        get => _os;
        set => this.RaiseAndSetIfChanged(ref _os, value);
    }
    string _os = "";

    public string Framework
    {
        get => _framework;
        set => this.RaiseAndSetIfChanged(ref _framework, value);
    }
    string _framework = "";

    [Column]
    public string Exe
    {
        get => _exe;
        set => this.RaiseAndSetIfChanged(ref _exe, value);
    }
    string _exe = "";

    [Column]
    public string Version
    {
        get => _version;
        set => this.RaiseAndSetIfChanged(ref _version, value);
    }
    string _version = "";

    [Column]
    public bool X64
    {
        get => _x64;
        set => this.RaiseAndSetIfChanged(ref _x64, value);
    }
    bool _x64 = true;

    public int Notify
    {
        get => _notify;
        set => this.RaiseAndSetIfChanged(ref _notify, value);
    }
    int _notify;
}

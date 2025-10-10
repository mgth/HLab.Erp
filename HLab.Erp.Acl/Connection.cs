using HLab.Erp.Data;
using HLab.Erp.Data.foreigners;
using NPoco;
using ReactiveUI;

namespace HLab.Erp.Acl;

//[Key("Id", AutoIncrement = false)]
//[SoftIncrementAttribut]
public class Connection : Entity
{
    public Connection()
    {
        _user = this.Foreign(e => e.UserId, e => e.User);
    }

    public int? UserId
    { 
        get => _user.Id;
        set => _user.SetId(value);    
    }

    [Ignore]
    public User? User
    { 
        get => _user.Value;
        set => UserId = value?.Id;    
    }
    readonly ForeignPropertyHelper<Connection,User> _user;

    public string Account
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "";

    public string Domain
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "";

    public string Workstation
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "";

    public string Os
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "";

    public string Framework
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "";

    [Column]
    public string Exe
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "";

    [Column]
    public string Version
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "";

    [Column]
    public bool X64
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public int Notify
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}

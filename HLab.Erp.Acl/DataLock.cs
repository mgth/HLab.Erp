using System;
using HLab.Erp.Data;
using NPoco;
using ReactiveUI;

namespace HLab.Erp.Acl;

public class DataLock : Entity
{
    public DataLock()
    {
        _user = Foreign(this, e => e.UserId, e => e.User);
    }

    public string EntityClass        {
        get => _entityClass;
        set => this.RaiseAndSetIfChanged(ref _entityClass, value);
    }
    string _entityClass = "";

    public int? EntityId
    {
        get => _entityId;
        set => this.RaiseAndSetIfChanged(ref _entityId, value);
    }
    int? _entityId;

    public int? UserId
    {
        get => _userId;
        set => this.RaiseAndSetIfChanged(ref _userId, value);
    }
    int? _userId;

    [Ignore]
    public User User
    {
        get => _user.Value;
        set => UserId = value.Id;
    }
    readonly ObservableAsPropertyHelper<User> _user;

    public string Code
    {
        get => _code;
        set => this.RaiseAndSetIfChanged(ref _code, value);
    }
    string _code = GetNewCode();

    public DateTime StartTime
    {
        get => _startTime;
        set => this.RaiseAndSetIfChanged(ref _startTime, value);
    }
    DateTime _startTime = DateTime.Now.ToUniversalTime();

    public DateTime HeartbeatTime
    {
        get => _heartbeatTime; 
        set => this.RaiseAndSetIfChanged(ref _heartbeatTime, value);
    }
    DateTime _heartbeatTime = DateTime.Now.ToUniversalTime();

    public void Heartbeat(int heartBeat)
    {
        HeartbeatTime = DateTime.Now.AddMilliseconds(heartBeat).ToUniversalTime();
    }

    const string Charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    static string GetNewCode()
    {
        var result = "";
        for (int i = 0; i < 10; i++)
            result += Charset[new Random().Next(Charset.Length)];
        return result;
    }
}

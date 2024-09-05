using System;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;
using ReactiveUI;

namespace HLab.Erp.Acl;

public class AuditTrail : Entity, IListableModel
{
    public AuditTrail()
    {
        _user = Foreign(this, e => e.UserId, e => e.User);
    }

    public string EntityClass 
    { 
        get => _entityClass;
        set => this.RaiseAndSetIfChanged(ref _entityClass, value);    
    }
    string _entityClass;

    public string EntityCaption
    { 
        get => _entityCaption;
        set => this.RaiseAndSetIfChanged(ref _entityCaption, value);    
    }
    string _entityCaption;

    public string UserCaption 
    { 
        get => _userCaption;
        set => this.RaiseAndSetIfChanged(ref _userCaption, value);    
    }
    string _userCaption;

    public int? EntityId
    { 
        get => _entityId;
        set => this.RaiseAndSetIfChanged(ref _entityId, value);    
    }
    int? _entityId;

    public DateTime TimeStamp
    { 
        get => _timeStamp.ToUniversalTime();
        set => this.RaiseAndSetIfChanged(ref _timeStamp, value.ToUniversalTime());    
    }
    DateTime _timeStamp;

    public string Motivation 
    { 
        get => _motivation;
        set => this.RaiseAndSetIfChanged(ref _motivation, value);    
    }
    string _motivation ;

    public string Log 
    { 
        get => _log;
        set => this.RaiseAndSetIfChanged(ref _log, value);    
    }
    string _log;

    public string IconPath  
    { 
        get => _iconPath;
        set => this.RaiseAndSetIfChanged(ref _iconPath, value);    
    }
    string _iconPath;

    public string Action  
    { 
        get => _action;
        set => this.RaiseAndSetIfChanged(ref _action, value);    
    }
    string _action;

    public int? UserId
    { 
        get => _userId;
        set => this.RaiseAndSetIfChanged(ref _userId, value);    
    }
    int? _userId;
    public User User
    { 
        get => _user.Value;
        set => UserId = value.Id;    
    }
    readonly ObservableAsPropertyHelper<User> _user;

    public bool? Failed 
    { 
        get => _failed;
        set => this.RaiseAndSetIfChanged(ref _failed, value);    
    }
    bool? _failed;

}

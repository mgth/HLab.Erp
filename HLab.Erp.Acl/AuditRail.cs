using System;
using HLab.Erp.Data;
using HLab.Mvvm.Application;

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
        set => SetAndRaise(ref _entityClass, value);    
    }
    string _entityClass;

    public string EntityCaption
    { 
        get => _entityCaption;
        set => SetAndRaise(ref _entityCaption, value);    
    }
    string _entityCaption;

    public string UserCaption 
    { 
        get => _userCaption;
        set => SetAndRaise(ref _userCaption, value);    
    }
    string _userCaption;

    public int? EntityId
    { 
        get => _entityId;
        set => SetAndRaise(ref _entityId, value);    
    }
    int? _entityId;

    public DateTime TimeStamp
    { 
        get => _timeStamp.ToUniversalTime();
        set => SetAndRaise(ref _timeStamp, value.ToUniversalTime());    
    }
    DateTime _timeStamp;

    public string Motivation 
    { 
        get => _motivation;
        set => SetAndRaise(ref _motivation, value);    
    }
    string _motivation ;

    public string Log 
    { 
        get => _log;
        set => SetAndRaise(ref _log, value);    
    }
    string _log;

    public string IconPath  
    { 
        get => _iconPath;
        set => SetAndRaise(ref _iconPath, value);    
    }
    string _iconPath;

    public string Action  
    { 
        get => _action;
        set => SetAndRaise(ref _action, value);    
    }
    string _action;

    public int? UserId
    { 
        get => _user.Id;
        set => _user.SetId(value);
    }
    User User
    { 
        get => _user.Value;
        set => UserId = value.Id;    
    }
    readonly ForeignPropertyHelper<AuditTrail,User> _user;

    public bool? Failed 
    { 
        get => _failed;
        set => SetAndRaise(ref _failed, value);    
    }
    bool? _failed;

}

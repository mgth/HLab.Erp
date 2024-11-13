using System;
using HLab.Base.ReactiveUI;
using HLab.Erp.Data;
using HLab.Erp.Data.foreigners;
using HLab.Mvvm.Application;

namespace HLab.Erp.Acl.AuditTrails;

public class AuditTrail : Entity, IListableModel
{
    public AuditTrail()
    {
        _user = this.Foreign( e => e.UserId, e => e.User);
    }

    public string EntityClass
    {
        get => _entityClass;
        set => this.SetAndRaise(ref _entityClass, value);
    }
    string _entityClass;

    public string EntityCaption
    {
        get => _entityCaption;
        set => this.SetAndRaise(ref _entityCaption, value);
    }
    string _entityCaption;

    public string UserCaption
    {
        get => _userCaption;
        set => this.SetAndRaise(ref _userCaption, value);
    }
    string _userCaption;

    public int? EntityId
    {
        get => _entityId;
        set => this.SetAndRaise(ref _entityId, value);
    }
    int? _entityId;

    public DateTime TimeStamp
    {
        get => _timeStamp.ToUniversalTime();
        set => this.SetAndRaise(ref _timeStamp, value.ToUniversalTime());
    }
    DateTime _timeStamp;

    public string Motivation
    {
        get => _motivation;
        set => this.SetAndRaise(ref _motivation, value);
    }
    string _motivation;

    public string Log
    {
        get => _log;
        set => this.SetAndRaise(ref _log, value);
    }
    string _log;

    public string IconPath
    {
        get => _iconPath;
        set => this.SetAndRaise(ref _iconPath, value);
    }
    string _iconPath;

    public string Action
    {
        get => _action;
        set => this.SetAndRaise(ref _action, value);
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
    readonly ForeignPropertyHelper<AuditTrail, User> _user;

    public bool? Failed
    {
        get => _failed;
        set => this.SetAndRaise(ref _failed, value);
    }
    bool? _failed;

}

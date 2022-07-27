using System;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl
{
    using H = HD<AuditTrail>;

    public class AuditTrail : Entity, IListableModel
    {
        public AuditTrail() => H.Initialize(this);

        public string EntityClass 
        { 
            get => _entityClass.Get();
            set => _entityClass.Set(value);    
        }

        readonly IProperty<string> _entityClass = H.Property<string>();
        public string EntityCaption
        { 
            get => _entityCaption.Get();
            set => _entityCaption.Set(value);    
        }

        readonly IProperty<string> _entityCaption = H.Property<string>();
        public string UserCaption 
        { 
            get => _userCaption.Get();
            set => _userCaption.Set(value);    
        }

        readonly IProperty<string> _userCaption = H.Property<string>();

        public int? EntityId
        { 
            get => _entityId.Get();
            set => _entityId.Set(value);    
        }

        readonly IProperty<int?> _entityId = H.Property<int?>();
        public DateTime TimeStamp
        { 
            get => _timeStamp.Get().ToUniversalTime();
            set => _timeStamp.Set(value.ToUniversalTime());    
        }

        readonly IProperty<DateTime> _timeStamp = H.Property<DateTime>();
        public string Motivation 
        { 
            get => _motivation.Get();
            set => _motivation.Set(value);    
        }

        readonly IProperty<string> _motivation = H.Property<string>();
        public string Log 
        { 
            get => _log.Get();
            set => _log.Set(value);    
        }

        readonly IProperty<string> _log = H.Property<string>();
        public string IconPath  
        { 
            get => _iconPath.Get();
            set => _iconPath.Set(value);    
        }

        readonly IProperty<string> _iconPath = H.Property<string>();

        public string Action  
        { 
            get => _action.Get();
            set => _action.Set(value);    
        }

        readonly IProperty<string> _action = H.Property<string>();

        public int? UserId
        { 
            get => _user.Id.Get();
            set => _user.Id.Set(value);    
        }

        public User User
        { 
            get => _user.Get();
            set => _user.Set(value);    
        }

        readonly IForeign<User> _user = H.Foreign<User>();
        public bool? Failed 
        { 
            get => _failed.Get();
            set => _failed.Set(value);    
        }

        readonly IProperty<bool?> _failed = H.Property<bool?>();

    }
}

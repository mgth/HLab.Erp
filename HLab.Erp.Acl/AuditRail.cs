﻿using HLab.DependencyInjection.Annotations;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;
using System;

namespace HLab.Erp.Acl
{
    public class AuditTrail : Entity<AuditTrail>
    {
        public string EntityClass 
        { 
            get => _entityClass.Get();
            set => _entityClass.Set(value);    
        }
        private IProperty<string> _entityClass = H.Property<string>();
        public string EntityCaption
        { 
            get => _entityCaption.Get();
            set => _entityCaption.Set(value);    
        }
        private IProperty<string> _entityCaption = H.Property<string>();
        public string UserCaption 
        { 
            get => _userCaption.Get();
            set => _userCaption.Set(value);    
        }
        private IProperty<string> _userCaption = H.Property<string>();

        public int? EntityId
        { 
            get => _entityId.Get();
            set => _entityId.Set(value);    
        }
        private IProperty<int?> _entityId = H.Property<int?>();
        public DateTime TimeStamp
        { 
            get => _timeStamp.Get();
            set => _timeStamp.Set(value);    
        }
        private IProperty<DateTime> _timeStamp = H.Property<DateTime>();
        public string Motivation 
        { 
            get => _motivation.Get();
            set => _motivation.Set(value);    
        }
        private IProperty<string> _motivation = H.Property<string>();
        public string Log 
        { 
            get => _log.Get();
            set => _log.Set(value);    
        }
        private IProperty<string> _log = H.Property<string>();
        public string IconPath  
        { 
            get => _iconPath.Get();
            set => _iconPath.Set(value);    
        }
        private IProperty<string> _iconPath = H.Property<string>();

        public string Action  
        { 
            get => _action.Get();
            set => _action.Set(value);    
        }
        private IProperty<string> _action = H.Property<string>();

        public int? UserId
        { 
            get => _user.Id.Get();
            set => _user.Id.Set(value);    
        }

        [Ignore]
        public User User
        { 
            get => _user.Get();
            set => _user.Set(value);    
        }
        private IForeign<User> _user = H.Foreign<User>();
        public bool? Failed 
        { 
            get => _failed.Get();
            set => _failed.Set(value);    
        }

        private IProperty<bool?> _failed = H.Property<bool?>();

    }
}
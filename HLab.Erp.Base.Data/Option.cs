using System;
using System.Collections.Generic;
using System.Text;
using HLab.Erp.Acl;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Base.Data
{
    public class Option : Entity<Option>
    {
        public int? UserId
        {
            get => _userId.Get();
            set => _userId.Set(value);
        }
        private readonly IProperty<int?> _userId = H.Property<int?>();

        [Ignore]
        public User User
        {
            get => _user.Get();
            set => UserId = value.Id;
        }
        private readonly IProperty<User> _user = H.Property<User>(c => c.Foreign(e => e.UserId));

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }
        private readonly IProperty<string> _name = H.Property<string>();

        public string Value
        {
            get => _value.Get();
            set => _value.Set(value);
        }
        private readonly IProperty<string> _value = H.Property<string>();
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl
{
    using H = HD<Profile>;

    public class Profile : Entity
    {
        public Profile() => H.Initialize(this);

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));
    }
}

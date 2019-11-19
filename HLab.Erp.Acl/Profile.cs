using System;
using System.Collections.Generic;
using System.Text;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Acl
{
    public class UserProfile : Entity<UserProfile>
    {
        public int? ProfileId
        {
            get => _profile.Id.Get();
            set => _profile.Id.Set(value);
        }

        [Ignore]
        public Profile Profile
        {
            get => _profile.Get();
            set => _profile.Set(value);
        }
        readonly IForeign<Profile> _profile = H.Foreign<Profile>();

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
        readonly IForeign<User> _user = H.Foreign<User>();
    }

    public class Profile : Entity<Profile>
    {
        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));
    }
}

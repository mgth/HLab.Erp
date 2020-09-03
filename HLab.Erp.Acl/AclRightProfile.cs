using System;
using System.Collections.Generic;
using System.Text;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Acl
{
    using H = HD<AclRightProfile>;
    public class AclRightProfile : Entity
    {
        public AclRightProfile() => H.Initialize(this);

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

        public int? AclRightId
        {
            get => _aclRight.Id.Get();
            set => _aclRight.Id.Set(value);
        }

        [Ignore]
        public AclRight AclRight
        {
            get => _aclRight.Get();
            set => _aclRight.Set(value);
        }
        readonly IForeign<AclRight> _aclRight = H.Foreign<AclRight>();
    }
}

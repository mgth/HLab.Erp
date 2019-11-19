using System.ComponentModel.DataAnnotations.Schema;

using HLab.Notify.Annotations;

using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Acl
{
    public class AclLink : Entity<AclLink>
    {
        public int? GroupId
        {
            get => _group.Id.Get();
            set => _group.Id.Set(value);
        }

        [Ignore]
        public AclNode Group
        {
            get => _group.Get();
            set => _group.Set(value);
        }
        readonly IForeign<AclNode> _group = H.Foreign<AclNode>();

        public int? MemberId
        {
            get => _member.Id.Get();
            set => _member.Id.Set(value);
        }

        [Ignore]
        public AclNode Member
        {
            get => _member.Get();
            set => _member.Set(value);
        }
        readonly IForeign<AclNode> _member = H.Foreign<AclNode>();
    }
}
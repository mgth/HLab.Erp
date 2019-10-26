using System.ComponentModel.DataAnnotations.Schema;

using HLab.Notify.Annotations;

using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Acl
{
    public class AclLink : Entity<AclLink>
    {
        [System.ComponentModel.DataAnnotations.Schema.Column]
        public int? GroupId
        {
            get => _groupId.Get();
            set => _groupId.Set(value);
        }

        private readonly IProperty<int?> _groupId = H.Property<int?>();

        [NotMapped]
        [TriggerOn(nameof(GroupId))]
        public AclNode Group
        {
            //get => E.GetForeign<AclNode>(()=>GroupId); set => GroupId = value.Id;
            get => _group.Get();
            set => _group.Set(value);
        }
        IProperty<AclNode> _group = H.Property<AclNode>();

        [System.ComponentModel.DataAnnotations.Schema.Column]
        public int? MemberId
        {
            get => _memberId.Get();
            set => _memberId.Set(value);
        }
        IProperty<int?> _memberId = H.Property<int?>();

        [Ignore]
        public AclNode Member
        {
            //get => E.GetForeign<AclNode>(() => MemberId); set => MemberId = value.Id;
            get => _member.Get();
            set => _member.Set(value);
        }
        IForeign<AclNode> _member = H.Foreign<AclNode>();
    }
}
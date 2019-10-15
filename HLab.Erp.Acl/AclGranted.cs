using System.ComponentModel.DataAnnotations.Schema;
using HLab.Erp.Data;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl
{
    public class AclGranted : Entity<AclGranted>
    {
        [Column]
        public bool Deny
        {
            get => _aclGranted.Get();
            set => _aclGranted.Set(value);
        }
        IProperty<bool> _aclGranted = H.Property<bool>();

        [Column]
        public string Right
        {
            get => _right.Get();
            set => _right.Set(value);
        }
        IProperty<string> _right = H.Property<string>(c => c.Default(""));

        [Column]

        public int AclNodeId
        {
            get => _aclNodeId.Get();
            set => _aclNodeId.Set(value);
        }
        IProperty<int> _aclNodeId = H.Property<int>(c => c.Default(-1));

        [NotMapped]
        [TriggerOn(nameof(AclNodeId))]
        public AclNode Member
        {
            get => _member.Get();
            set => _member.Set(value);
            //get => E.GetForeign<AclNode>(()=>AclNodeId);
            //set => AclNodeId = value.Id;
        }
        IProperty<AclNode> _member = H.Property<AclNode>();

        [Column]
        public int? AclListId
        {
            get => _aclListId.Get();
            set => _aclListId.Set(value);
        }
        IProperty<int?> _aclListId = H.Property<int?>();

        [NotMapped]
        [TriggerOn(nameof(AclListId))]
        public AclList AclList
        {
            //get => E.GetForeign<AclList>(() => AclListId);
            //set => AclListId = value.Id;
            get => _aclList.Get();
            set => _aclList.Set(value);
        }
        IProperty<AclList> _aclList = H.Property<AclList>();

    }
}

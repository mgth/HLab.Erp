using System.ComponentModel.DataAnnotations.Schema;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl
{
    using H = HD<AclList>;

    public class AclList : Entity
    {
        public AclList() => H.Initialize(this);

        [Column]
        public string Target
        {
            get => _target.Get();
            set => _target.Set(value);
        }

        readonly IProperty<string> _target = H.Property<string>(c => c.Default(""));

        [Column]
        public string ParentId
        {
            get => _parentId.Get();
            set => _parentId.Set(value);
        }

        private readonly IProperty<string> _parentId = H.Property<string>(c => c.Default(""));

        [NotMapped][Import]
        public ObservableQuery<AclGranted> Granted
        {
            get => _granted.Get();
            set => _granted.Set(value.AddFilter("", ()=>n => n.ToNodeId == Id)
                    .FluentUpdate());
        }

        private readonly IProperty<ObservableQuery<AclGranted>> _granted = H.Property<ObservableQuery<AclGranted>>();

        //[TriggerOn(nameof(ParentId))]
        //public AclList Parent
        //{
        //    get => E.GetForeign<AclList>(ParentId); set => ParentId = value.Id;
        //}

        [Column]
        public bool Inherit
        {
            get => _inherit.Get();
            set => _inherit.Set(value);
        }

        private readonly IProperty<bool> _inherit = H.Property<bool>(c => c.Default(true));

    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Acl
{
    public class AclNode : Entity<AclNode>
    {
        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));
        public string TargetClass
        {
            get => _targetClass.Get();
            set => _targetClass.Set(value);
        }

        private readonly IProperty<string> _targetClass = H.Property<string>(c => c.Default(""));
        public int? TargetId
        {
            get => _targetId.Get();
            set => _targetId.Set(value);
        }

        private readonly IProperty<int?> _targetId = H.Property<int?>();

        [Import][Ignore]
        public ObservableQuery<AclLink> Members
        {
            get => _members.Get();
            set => _members.Set(value.AddFilter("", ()=>n => n.GroupId == Id)
                    .FluentUpdate());
        }
        private readonly IProperty<ObservableQuery<AclLink>> _members = H.Property<ObservableQuery<AclLink>>();

        [Import][Ignore]
        public ObservableQuery<AclLink> Groups
        {
            get => _groups.Get();
            set => _groups.Set(value.AddFilter("", ()=>n => n.MemberId == Id)
                    .FluentUpdate());
        }

        private readonly IProperty<ObservableQuery<AclLink>> _groups = H.Property<ObservableQuery<AclLink>>();

        public bool Grant(AclRight right, AclNode target)
        {
            DataService.Add<AclGranted>(
                t =>
                {
                    t.ToNodeId = Id;
                    t.Right = right;
                    t.OnNodeId = target.Id;
                }
            );

            return true;
        }

        public IEnumerable<int?> GetGroups()
        {
            foreach (var g in Groups)
            {
                yield return g.Group.Id;
                foreach (var gg in g.Group.GetGroups())
                {
                    yield return gg;
                }
            }
        }

        public async Task<bool> IsGranted(AclRight right, AclNode target)
        {
            var groups = GetGroups().ToList();
            var targets = target.GetGroups().ToList();

            var grants = await DataService.FetchWhereAsync<AclGranted>(e => 
                    groups.Contains(e.ToNodeId) && 
                    targets.Contains(e.OnNodeId)
                ,null).ConfigureAwait(false);

            if (grants.Count == 0) return false;
            if (grants.Any(e => e.Deny)) return false;

            return true;
        }
    }
}

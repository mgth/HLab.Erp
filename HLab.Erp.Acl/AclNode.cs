using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Notify;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl
{
    public class AclNode : Entity<AclNode>
    {
        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }
        IProperty<string> _name = H.Property<string>(c => c.Default(""));

        [Import][NotMapped]
        public ObservableQuery<AclLink> Members
        {
            get => _members.Get();
            set => _members.Set(value.AddFilter("", ()=>n => n.GroupId == Id)
                    .FluentUpdate());
        }
        IProperty<ObservableQuery<AclLink>> _members = H.Property<ObservableQuery<AclLink>>();

        [Import][NotMapped]
        public ObservableQuery<AclLink> Groups
        {
            get => _groups.Get();
            set => _groups.Set(value.AddFilter("", ()=>n => n.MemberId == Id)
                    .FluentUpdate());
        }
        IProperty<ObservableQuery<AclLink>> _groups = H.Property<ObservableQuery<AclLink>>();

        public bool Grant(string right, IAclTarget target)
        {
            // TODO
            //E.DbService.Add<AclGranted>(
            //    t =>
            //    {
            //        t.AclNodeId = Id;
            //        t.Right = right;
            //        t.AclListId = target.GetOrAddAclList(E.DbService).Id;
            //    }
            //);

            return true;
        }

        public IEnumerable<int> GetGroups()
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

        public bool IsGranted(string right, IAclTarget target)
        {
            var groups = GetGroups().ToList();
            return true;
            // TODO : return target.IsGranted(right, groups, E.DbService);
        }
    }
}

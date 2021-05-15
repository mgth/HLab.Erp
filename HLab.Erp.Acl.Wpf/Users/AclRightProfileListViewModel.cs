using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;

namespace HLab.Erp.Acl.Users
{
    public class AclRightProfileListViewModel : EntityListViewModel<AclRightProfile>
    {
        public AclRightProfileListViewModel(AclRight right) : base(c => c
            .StaticFilter(e => e.AclRightId == right.Id)
            .Column()
            .Header("{Name}")
            .Content(s => s.Profile.Name)
        )
        {
            OpenAction = target => Erp.Docs.OpenDocumentAsync(target.Profile);
        }

        public AclRightProfileListViewModel(Profile profile) : base(c => c
           .StaticFilter(e => e.ProfileId == profile.Id)
           .Column()
           .Header("{Name}")
           .Content(s => s.AclRight.Caption)
        )
        {
            OpenAction = target => { };
        }

    }
}
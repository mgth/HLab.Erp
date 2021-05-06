using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;

namespace HLab.Erp.Acl.Users
{
    public class AclRightProfileListViewModel : EntityListViewModel<AclRightProfile>
    {
        public AclRightProfileListViewModel(AclRight right) : base(c => c
            .Column()
            .Header("{Name}").Content(s => s.Profile.Name)
        )
        {
            OpenAction = target => Erp.Docs.OpenDocumentAsync(target.Profile);
            List.AddFilter(() => e => e.AclRightId == right.Id);
        }

        public AclRightProfileListViewModel(Profile profile) : base (c => c
            .Column().Header("{Name}").Content(s => s.AclRight.Caption)
        )
        {
            OpenAction = target => { };
            List.AddFilter(() => e => e.ProfileId == profile.Id);
        }

    }
}
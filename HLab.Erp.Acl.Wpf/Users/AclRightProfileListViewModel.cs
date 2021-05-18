using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;

namespace HLab.Erp.Acl.Users
{
    public class ProfilesAclRightListViewModel : EntityListViewModel<AclRightProfile>
    {
        public ProfilesAclRightListViewModel(AclRight right) : base(c => c
            .StaticFilter(e => e.AclRightId == right.Id)
            .Column()
            .Header("{Name}")
            .Content(s => s.Profile.Name)
        )
        {
            OpenAction = target => Erp.Docs.OpenDocumentAsync(target.Profile);
        }


    }
}
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;

namespace HLab.Erp.Acl.Users
{
    public class ProfilesAclRightListViewModel : EntityListViewModel<AclRightProfile>
    {
        public ProfilesAclRightListViewModel(AclRight right) : base(c => c
            .StaticFilter(e => e.AclRightId == right.Id)
            .Column("Name")
            .Header("{Name}")
            .Content(s => s.Profile.Name)
        )
        {
            OpenAction = target => Erp.Docs.OpenDocumentAsync(target.Profile);
        }


    }
}
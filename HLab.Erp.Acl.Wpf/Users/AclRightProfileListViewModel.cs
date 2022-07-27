using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;

namespace HLab.Erp.Acl.Users
{
    public class ProfilesAclRightListViewModel : Core.EntityLists.EntityListViewModel<AclRightProfile>
    {
        public ProfilesAclRightListViewModel(Injector i, AclRight right) : base(i, c => c
            .StaticFilter(e => e.AclRightId == right.Id)
            .Column("Name")
            .Header("{Name}")
            .Content(s => s.Profile.Name)
        )
        {
            OpenAction = target => i.Erp.Docs.OpenDocumentAsync(target.Profile);
        }


    }
}
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Mvvm.Application;
using HLab.Mvvm.Application.Documents;

namespace HLab.Erp.Acl.Users;

public class ProfilesAclRightListViewModel : Core.EntityLists.EntityListViewModel<AclRightProfile>
{
    public ProfilesAclRightListViewModel(IDocumentService docs, Injector i, AclRight right) : base(i, c => c
        .StaticFilter(e => e.AclRightId == right.Id)
        .Column("Name")
        .Header("{Name}")
        .Content(s => s.Profile.Name)
    )
    {
        OpenAction = target => docs.OpenDocumentAsync(target.Profile);
    }


}
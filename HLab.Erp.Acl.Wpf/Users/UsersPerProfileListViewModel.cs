using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using System;
using HLab.Mvvm.Application;

namespace HLab.Erp.Acl.Users
{
    public class UsersPerProfileListViewModel : Core.EntityLists.EntityListViewModel<UserProfile>
    {
        public UsersPerProfileListViewModel(Injector i, Profile profile) : base(i, c => c
            .StaticFilter(e => e.ProfileId == profile.Id)
            .Column("Name")
            .Header("{Name}")
            .Content(s => s.User.Caption)
        )
        {
            OpenAction = target => i.Erp.Docs.OpenDocumentAsync(target.User);
        }

        protected override bool CanExecuteAdd(Action<string> errorAction) => Injected.Erp.Acl.IsGranted(errorAction, AclRights.ManageProfiles);
        protected override bool CanExecuteDelete(UserProfile profile, Action<string> errorAction) => Injected.Erp.Acl.IsGranted(errorAction, AclRights.ManageProfiles);
    }
}
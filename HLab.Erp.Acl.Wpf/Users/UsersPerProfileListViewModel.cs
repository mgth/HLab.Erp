using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using System;

namespace HLab.Erp.Acl.Users
{
    public class UsersPerProfileListViewModel : EntityListViewModel<UserProfile>
    {
        public UsersPerProfileListViewModel(Profile profile) : base(c => c
            .StaticFilter(e => e.ProfileId == profile.Id)
            .Column()
            .Header("{Name}")
            .Content(s => s.User.Caption)
        )
        {
            OpenAction = target => Erp.Docs.OpenDocumentAsync(target.User);
        }

        protected override bool CanExecuteAdd(Action<string> errorAction) => Erp.Acl.IsGranted(errorAction, AclRights.ManageProfiles);
        protected override bool CanExecuteDelete(UserProfile profile, Action<string> errorAction) => Erp.Acl.IsGranted(errorAction, AclRights.ManageProfiles);
    }
}
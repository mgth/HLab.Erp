using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;

namespace HLab.Erp.Acl.Users
{
    public class UserProfileListViewModel : EntityListViewModel<UserProfile>
    {
        public UserProfileListViewModel(User user) : base(c => c
            .Column()
            .Header("{Name}")
            .Content(s => s.Profile.Name)
        )
        {
            OpenAction = target => Erp.Docs.OpenDocumentAsync(target.Profile);

            List.AddFilter(() => e => e.UserId == user.Id);
        }
        public UserProfileListViewModel(Profile profile) : base(c => c
            .Column()
            .Header("{Name}")
            .Content(s => s.User.Caption)
        )
        {
            OpenAction = target => Erp.Docs.OpenDocumentAsync(target.User);

            List.AddFilter(() => e => e.ProfileId == profile.Id);
        }

    }
}
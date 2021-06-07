using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;

namespace HLab.Erp.Acl.Users
{
    public class ProfilesPerUserListViewModel : EntityListViewModel<UserProfile>
    {
        public ProfilesPerUserListViewModel(User user) : base(c => c
            .StaticFilter(e => e.UserId == user.Id)
            .Column()
            .Header("{Name}")
            .Content(s => s.Profile.Name)
        )
        {
            OpenAction = target => Erp.Docs.OpenDocumentAsync(target.Profile);
        }

        //public UserProfileListViewModel(Profile profile) : base(c => c
        //    .StaticFilter(e => e.ProfileId == profile.Id)
        //    .Column()
        //    .Header("{Name}")
        //    .Content(s => s.User.Caption)
        //)
        //{
        //    OpenAction = target => Erp.Docs.OpenDocumentAsync(target.User);
        //}
    }
}
using HLab.Erp.Core.EntityLists;

namespace HLab.Erp.Acl.Users
{
    public class ListUserProfileViewModel : EntityListViewModel<UserProfile>
    {
        public ListUserProfileViewModel(User user)
        {
            OpenAction = target => _docs.OpenDocumentAsync(target.Profile);

            Columns.Configure(c => c
                .Column.Header("{Name}").Content(s => s.Profile.Name)
            );

            List.AddFilter(() => e => e.UserId == user.Id);

            List.Update();
        }        
        public ListUserProfileViewModel(Profile profile)
        {
            OpenAction = target => _docs.OpenDocumentAsync(target.User);

            Columns.Configure(c => c
                        .Column.Header("{Name}").Content(s => s.User.Caption)
            );

            List.AddFilter(() => e => e.ProfileId == profile.Id);

            List.Update();
        }        
    }
}
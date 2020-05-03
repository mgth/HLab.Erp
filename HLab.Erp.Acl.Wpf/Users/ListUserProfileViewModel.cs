using HLab.Erp.Core.EntityLists;

namespace HLab.Erp.Acl.Users
{
    public class ListUserProfileViewModel : EntityListViewModel<ListUserProfileViewModel,UserProfile>
    {
        public ListUserProfileViewModel(User user)
        {
            OpenAction = target => _docs.OpenDocumentAsync(target.Profile);

            Columns
                .Column("{Name}", s => s.Profile.Name);

            List.AddFilter(() => e => e.UserId == user.Id);

            List.UpdateAsync();
        }        
        public ListUserProfileViewModel(Profile profile)
        {
            OpenAction = target => _docs.OpenDocumentAsync(target.User);

            Columns
                .Column("{Name}", s => s.User.Caption);

            List.AddFilter(() => e => e.ProfileId == profile.Id);

            List.UpdateAsync();
        }        
    }
}
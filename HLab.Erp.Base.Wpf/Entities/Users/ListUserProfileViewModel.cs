using HLab.Erp.Acl;
using HLab.Erp.Core.EntityLists;

namespace HLab.Erp.Base.Wpf.Entities.Users
{
    public class ListAclRightProfileViewModel : EntityListViewModel<ListAclRightProfileViewModel,AclRightProfile>
    {
        public ListAclRightProfileViewModel(AclRight right)
        {
            OpenAction = target => _docs.OpenDocumentAsync(target.Profile);

            Columns
                .Column("{Name}", s => s.Profile.Name);

            List.AddFilter(() => e => e.AclRightId == right.Id);

            List.UpdateAsync();


        }
        public ListAclRightProfileViewModel(Profile profile)
        {
            OpenAction = target => { };
            Columns
                .Column("{Name}", s => s.AclRight.Caption);

            List.AddFilter(() => e => e.ProfileId == profile.Id);

            List.UpdateAsync();
        }        
    }


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
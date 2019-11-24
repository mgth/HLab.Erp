using HLab.Erp.Acl;
using HLab.Erp.Core.EntityLists;

namespace HLab.Erp.Base.Wpf.Entities.Users
{
    public class ListAclRightProfileViewModel : EntityListViewModel<ListAclRightProfileViewModel,AclRightProfile>
    {
        public ListAclRightProfileViewModel(AclRight right)
        {
            Columns
                .Column("Name", s => s.Profile.Name);

            List.AddFilter(() => e => e.AclRightId == right.Id);

            List.Update();
        }        
        public ListAclRightProfileViewModel(Profile profile)
        {
            Columns
                .Column("Name", s => s.AclRight.Caption);

            List.AddFilter(() => e => e.ProfileId == profile.Id);

            List.Update();
        }        
    }


    public class ListUserProfileViewModel : EntityListViewModel<ListUserProfileViewModel,UserProfile>
    {
        public ListUserProfileViewModel(User user)
        {
            Columns
                .Column("Name", s => s.Profile.Name);

            List.AddFilter(() => e => e.UserId == user.Id);

            List.Update();
        }        
        public ListUserProfileViewModel(Profile profile)
        {
            Columns
                .Column("Name", s => s.User.Caption);

            List.AddFilter(() => e => e.ProfileId == profile.Id);

            List.Update();
        }        
    }
}
using HLab.Erp.Acl;
using HLab.Erp.Core.ViewModels.EntityLists;

namespace HLab.Erp.Base.Wpf.Entities.Users
{
    public class ListProfileViewModel : EntityListViewModel<ListProfileViewModel,Profile>
    {
        public ListProfileViewModel(User user)
        {
            Columns
                .Column("Name", s => s.Name);

//            List.AddFilter(() => e => e.UserId == user.Id);

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
    }

    public class ListUserViewModel : EntityListViewModel<ListUserViewModel,User>
    {
        public ListUserViewModel()
        {
            Columns
                .Column("Name", s => s.Name)
                .Column("First Name", s => s.FirstName)
                .Column("Login", s => s.Login)
                .Column("Initials", s => s.Initials);

            List.Update();
        }

    }
}

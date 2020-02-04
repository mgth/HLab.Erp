using HLab.Erp.Acl;
using HLab.Erp.Core.EntityLists;

namespace HLab.Erp.Base.Wpf.Entities.Profiles
{
    public class ListProfileViewModel : EntityListViewModel<ListProfileViewModel,Profile>
    {
        public string Title => "{Profiles}";
        public string IconPath => "Icons/Entities/Profile";

        public ListProfileViewModel(User user)
        {
            Columns
                .Column("{Name}", s => s.Name);

//            List.AddFilter(() => e => e.UserId == user.Id);

            List.UpdateAsync();
        }        
        public ListProfileViewModel()
        {
            AddAllowed = true;
            DeleteAllowed = true;

            Columns
                .Column("{Name}", s => s.Name);

//            List.AddFilter(() => e => e.UserId == user.Id);

            List.UpdateAsync();
        }        
    }
}
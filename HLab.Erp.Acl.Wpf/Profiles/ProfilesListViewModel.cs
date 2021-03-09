using HLab.Erp.Core.EntityLists;

namespace HLab.Erp.Acl.Profiles
{
    public class ProfilesListViewModel : EntityListViewModel<Profile>
    {
        public ProfilesListViewModel(User user)
        {
            Columns.Configure(c => c
                .Column.Header("{Name}").Content(s => s.Name)
            );

            //List.AddFilter(e => e. == user.Id);

            List.UpdateAsync();
        }        
        public ProfilesListViewModel()
        {
            AddAllowed = true;
            DeleteAllowed = true;

            Columns.Configure(c => c
                .Column.Header("{Name}").Content(s => s.Name)
            );

//            List.AddFilter(() => e => e.UserId == user.Id);

            List.UpdateAsync();
        }        
    }
}
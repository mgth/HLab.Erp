using HLab.Erp.Core.EntityLists;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Acl.Users
{
    public class UsersListViewModel : EntityListViewModel<User>, IMvvmContextProvider
    {
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        public UsersListViewModel()
        {
            DeleteAllowed = true;
            AddAllowed = true;

            Columns.Configure(c => c
                    .Column.Header("{First Name}").Content(u => u.FirstName)
                    .Column.Header("{Name}").Content(u=>u.Name)
                    .Column.Header("{Login}").Content(u=>u.Login)
                    .Column.Header("{Function}").Content(u=>u.Function)
                    .Column.Header("{Initials}").Content(u=>u.Initials)
                
                );

            List.UpdateAsync();
        }
    }
}

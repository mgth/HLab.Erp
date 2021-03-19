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
                    .Column
                    .Header("{First Name}").Width(150)
                    .Content(u => u.FirstName)
                    .Column
                    .Header("{Name}").Width(100)
                    .Content(u=>u.Name)
                    .Column
                    .Header("{Login}").Width(100)
                    .Content(u=>u.Login)
                    .Column
                    .Header("{Function}").Width(250)
                    .Content(u=>u.Function)
                    .Column
                    .Header("{Initials}").Width(70)
                    .Content(u=>u.Initials)
                
                );

            List.Update();
        }
    }
}

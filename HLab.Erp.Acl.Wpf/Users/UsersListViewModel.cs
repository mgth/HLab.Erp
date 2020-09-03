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

            Columns
                .Column("{First Name}", u => u.FirstName)
                .Column("{Name}", u=>u.Name)
                .Column("{Login}", u=>u.Login)
                .Column("{Function}", u=>u.Function)
                .Column("{Initials}", u=>u.Initials)
                ;

            List.UpdateAsync();
        }
    }
}

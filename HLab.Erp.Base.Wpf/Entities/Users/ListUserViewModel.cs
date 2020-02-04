using System;
using HLab.Erp.Acl;
using HLab.Erp.Core.EntityLists;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf.Entities.Users
{
    public class ListUserViewModel : EntityListViewModel<ListUserViewModel,User>, IMvvmContextProvider
    {
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        public String Title => "{Users}";
        public string Icon => "Icons/entities/Users";

        public ListUserViewModel()
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

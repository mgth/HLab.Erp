using System;
using System.Reflection;
using System.Windows;
using HLab.Erp.Acl;
using HLab.Erp.Core.Wpf.ViewModels;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Module
{
    public class ListUsersViewModel : EntityListViewModel<ListUsersViewModel, User>, IMvvmContextProvider
    {
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        public String Title => "Users";
        public string Icon => "Icons/Users";

        public ListUsersViewModel()
        {
            Columns
                .Column("^First Name", u => u.FirstName)
                .Column("^Name", u=>u.Name)
                .Column("^Login", u=>u.Login)
                .Column("^Function", u=>u.Function)
                .Column("^Initials", u=>u.Initials)

                ;

            List.Update();
        }
    }
}

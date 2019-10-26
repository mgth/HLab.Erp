using System;
using System.Collections.Generic;
using System.Text;
using HLab.Erp.Acl;
using HLab.Erp.Core.ViewModels;

namespace HLab.Erp.Base.Wpf
{
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

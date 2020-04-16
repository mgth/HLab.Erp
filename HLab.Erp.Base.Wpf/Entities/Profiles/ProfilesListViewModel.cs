﻿using HLab.Erp.Acl;
using HLab.Erp.Core.EntityLists;

namespace HLab.Erp.Base.Wpf.Entities.Profiles
{
    public class ProfilesListViewModel : EntityListViewModel<ProfilesListViewModel,Profile>
    {
        public ProfilesListViewModel(User user)
        {
            Columns
                .Column("{Name}", s => s.Name);

            //List.AddFilter(e => e. == user.Id);

            List.UpdateAsync();
        }        
        public ProfilesListViewModel()
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
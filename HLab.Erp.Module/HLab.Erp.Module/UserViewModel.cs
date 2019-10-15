using System;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Data;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Module
{
    public class UserViewModel : EntityViewModel<UserViewModel,User>
    {
    }

    public class UserViewModelDesign
    {
        public User Model { get; } = new User
        {
            FirstName = "Gaston",
            Name = "Ouedraogo",
            Login = "g.ouedra",
            Function = "Technicien"
        };
    }
}

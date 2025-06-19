using System;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Acl.Users;

public class UsersListViewModel : Core.EntityLists.EntityListViewModel<User>, IMvvmContextProvider
{
    public class Bootloader : NestedBootloader
    {
        public override string MenuPath => "param";
    }

    public void ConfigureMvvmContext(IMvvmContext ctx)
    {
    }

    readonly IAclService _acl;
    protected override bool AddCanExecute(Action<string> errorAction) => _acl?.IsGranted(AclRights.ManageUser)??false;

    protected override bool DeleteCanExecute(User inn, Action<string> errorAction) => _acl?.IsGranted(AclRights.ManageUser)??false;

    public UsersListViewModel(Injector i, IAclService acl) : base(i, c => c
        .Column("FirstName")
        .Header("{First Name}").Width(150)
        .Link(u => u.FirstName)
        .Filter()

        .Column("Name")
        .Header("{Name}").Width(100)
        .Link(u => u.Name)
        .Filter()

        .Column("Login")
        .Header("{Login}").Width(100)
        .Link(u => u.Username)
        .Filter()

        .Column("Function")
        .Header("{Function}").Width(250)
        .Link(u => u.Function)
        .Filter()

        .Column("Initials")
        .Header("{Initials}").Width(70)
        .Link(u => u.Initials)
        .Filter()
    )
    {
        _acl = acl;
    }

}
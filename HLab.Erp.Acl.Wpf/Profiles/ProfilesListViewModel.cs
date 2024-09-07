using HLab.Erp.Core;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using System;

namespace HLab.Erp.Acl.Profiles;

public class ProfilesListViewModel : Core.EntityLists.EntityListViewModel<Profile>
{
    public class Bootloader : NestedBootloader
    {
        public override string MenuPath => "param";
    }

    readonly IAclService _acl;

    public ProfilesListViewModel(IAclService acl, Injector i) : base(i, c => c
        .Column("Name")
        .Header("{Name}")
        .Width(100).Content(s => s.Name)
    )
    {
        _acl = acl;
    }

    protected override bool AddCanExecute(Action<string> errorAction) => _acl.IsGranted(errorAction, AclRights.ManageProfiles);
    protected override bool DeleteCanExecute(Profile profile, Action<string> errorAction) => _acl.IsGranted(errorAction, AclRights.ManageProfiles);
}
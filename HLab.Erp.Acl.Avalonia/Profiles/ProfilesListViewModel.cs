using HLab.Erp.Core;
using HLab.Erp.Core.ListFilterConfigurators;
using System;
using HLab.Erp.Core.EntityLists;

namespace HLab.Erp.Acl.Profiles;

public class ProfilesListViewModel(IAclService acl, EntityListViewModel<Profile>.Injector i)
   : EntityListViewModel<Profile>(i, c => c
      .Column("Name")
      .Header("{Name}")
      .Width(100).Content(s => s.Name))
{
    public class Bootloader : NestedBootloader
    {
        public override string MenuPath => "param";
    }

    protected override bool AddCanExecute(Action<string> errorAction) => acl?.IsGranted(errorAction, AclRights.ManageProfiles)??false;
    protected override bool DeleteCanExecute(Profile profile, Action<string> errorAction) => acl?.IsGranted(errorAction, AclRights.ManageProfiles) ?? false;
}
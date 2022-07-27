using HLab.Erp.Core;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using System;

namespace HLab.Erp.Acl.Profiles
{
    public class ProfilesListViewModel : Core.EntityLists.EntityListViewModel<Profile>
    {
        public class Bootloader : NestedBootloader
        {
            public override string MenuPath => "param";
        }

        public ProfilesListViewModel(Injector i) : base(i, c => c
                .Column("Name")
                .Header("{Name}")
                .Width(100).Content(s => s.Name)
        )
        {
        }

        protected override bool CanExecuteAdd(Action<string> errorAction) => Injected.Erp.Acl.IsGranted(errorAction, AclRights.ManageProfiles);
        protected override bool CanExecuteDelete(Profile profile, Action<string> errorAction) => Injected.Erp.Acl.IsGranted(errorAction, AclRights.ManageProfiles);
    }
}
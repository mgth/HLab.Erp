using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using System;

namespace HLab.Erp.Acl.Profiles
{
    public class ProfilesListViewModel : EntityListViewModel<Profile>
    {
        public class Bootloader : NestedBootloader
        {
            public override string MenuPath => "param";
        }

        public ProfilesListViewModel() : base(c => c
                .Column()
                .Header("{Name}")
                .Width(100).Content(s => s.Name)
        )
        {
        }

        protected override bool CanExecuteAdd(Action<string> errorAction) => Erp.Acl.IsGranted(errorAction, AclRights.ManageProfiles);
        protected override bool CanExecuteDelete(Profile profile, Action<string> errorAction) => Erp.Acl.IsGranted(errorAction, AclRights.ManageProfiles);
    }
}
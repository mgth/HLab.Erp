using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;

namespace HLab.Erp.Acl.Profiles
{
    public class ProfilesListViewModel : EntityListViewModel<Profile>
    {
        public class Bootloader : NestedBootloader
        {
            public override string MenuPath => "param";
        }

        public ProfilesListViewModel(User user) : base(c => c
            // TODO .StaticFilter(e => e.Id == user.Id)
            .Column()
            .Header("{Name}")
            .Width(100)
            .Link(s => s.Name)
        )
        {
        }        

        public ProfilesListViewModel() : base(c => c
                .Column()
                .Header("{Name}")
                .Width(100).Content(s => s.Name)
// TODO                 .AddAllowed()
// TODO                .DeleteAllowed()
        )
        {
        }

    }
}
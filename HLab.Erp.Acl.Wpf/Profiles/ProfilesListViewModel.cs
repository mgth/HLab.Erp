using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;

namespace HLab.Erp.Acl.Profiles
{
    public class ProfilesListViewModel : EntityListViewModel<Profile>
    {
        public class Bootloader : NestedBootloader
        {
            public override string MenuPath => "param";
        }

        public ProfilesListViewModel(User user) : base(c => c
                .Column()
                    .Header("{Name}").Width(100)
                    .Content(s => s.Name)
        
        )
        {
            // TODO : List.AddFilter(e => e.User == user.Id);
        }        

        public ProfilesListViewModel() : base(c => c
            .AddAllowed()
            .DeleteAllowed()
            .Column()
                    .Header("{Name}")
                    .Width(100)
                    .Content(s => s.Name)
        )
        {
        }

    }
}
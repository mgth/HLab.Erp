using HLab.Erp.Core;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Acl.Users
{
    public class UsersListViewModel : EntityListViewModel<User>, IMvvmContextProvider
    {
        public class Bootloader : NestedBootloader
        {
            public override string MenuPath => "param";
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        public UsersListViewModel() : base(c => c
// TODO :                .DeleteAllowed()
//                .AddAllowed()
            .Column()
            .Header("{First Name}").Width(150)
            .Link(u => u.FirstName)
            .Filter()

            .Column()
            .Header("{Name}").Width(100)
            .Link(u => u.Name)
            .Filter()

            .Column()
            .Header("{Login}").Width(100)
            .Link(u => u.Login)
            .Filter()

            .Column()
            .Header("{Function}").Width(250)
            .Link(u => u.Function)
            .Filter()

            .Column()
            .Header("{Initials}").Width(70)
            .Link(u => u.Initials)
            .Filter()
        )
        {
        }

    }
}

using HLab.Erp.Core.EntityLists;

namespace HLab.Erp.Acl.Profiles
{
    public class ProfilesListViewModel : EntityListViewModel<Profile>
    {
        public ProfilesListViewModel Configure(User user)
        {
            Columns.Configure(c => c
                .Column
                    .Header("{Name}").Width(100)
                    .Content(s => s.Name)
            );

            //List.AddFilter(e => e. == user.Id);

            List.Update();

            return this;
        }        

        protected override void Configure()
        {
            AddAllowed = true;
            DeleteAllowed = true;

            Columns.Configure(c => c
                .Column
                    .Header("{Name}")
                    .Width(100)
                    .Content(s => s.Name)
            );

//            List.AddFilter(() => e => e.UserId == user.Id);

            List.Update();
        }

    }
}
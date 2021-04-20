using HLab.Erp.Core.EntityLists;

namespace HLab.Erp.Acl.Users
{
    public class AclRightProfileListViewModel : EntityListViewModel<AclRightProfile>
    {
        public AclRightProfileListViewModel Configure(AclRight right)
        {
            OpenAction = target => Erp.Docs.OpenDocumentAsync(target.Profile);

            Columns.Configure( c => c.
                Column
                .Header("{Name}")
                .Content(s => s.Profile.Name)
            );

            List.AddFilter(() => e => e.AclRightId == right.Id);

            List.Update();

            return this;
        }

        public AclRightProfileListViewModel Configure(Profile profile)
        {
            OpenAction = target => { };
            Columns.Configure( c=> c
                .Column.Header("{Name}")
                .Content(s => s.AclRight.Caption)
            );

            List.AddFilter(() => e => e.ProfileId == profile.Id);

            List.Update();

            return this;
        }

        protected override void Configure()
        {
        }
    }
}
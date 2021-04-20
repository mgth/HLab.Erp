using HLab.Erp.Core.EntityLists;

namespace HLab.Erp.Acl.Users
{
    public class ListUserProfileViewModel : EntityListViewModel<UserProfile>
    {
        public ListUserProfileViewModel Configure(User user)
        {
            OpenAction = target => Erp.Docs.OpenDocumentAsync(target.Profile);

            Columns.Configure(c => c
                .Column.Header("{Name}").Content(s => s.Profile.Name)
            );

            List.AddFilter(() => e => e.UserId == user.Id);

            List.Update();

            return this;
        }        
        public ListUserProfileViewModel Configure(Profile profile)
        {
            OpenAction = target => Erp.Docs.OpenDocumentAsync(target.User);

            Columns.Configure(c => c
                        .Column.Header("{Name}").Content(s => s.User.Caption)
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
using HLab.Erp.Core.EntityLists;

namespace HLab.Erp.Acl.Users
{
    public class AclRightProfileListViewModel : EntityListViewModel<AclRightProfile>
    {
        public AclRightProfileListViewModel(AclRight right)
        {
            OpenAction = target => _docs.OpenDocumentAsync(target.Profile);

            Columns
                .Column("{Name}", s => s.Profile.Name);

            List.AddFilter(() => e => e.AclRightId == right.Id);

            List.UpdateAsync();


        }
        public AclRightProfileListViewModel(Profile profile)
        {
            OpenAction = target => { };
            Columns
                .Column("{Name}", s => s.AclRight.Caption);

            List.AddFilter(() => e => e.ProfileId == profile.Id);

            List.UpdateAsync();
        }        
    }
}
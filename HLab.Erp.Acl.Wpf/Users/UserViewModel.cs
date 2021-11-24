using System;
using System.Linq;
using System.Windows.Input;
using HLab.Erp.Acl.Profiles;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl.Users
{
    using H = H<UserViewModel>;

    public class UserViewModel: ListableEntityViewModel<User>
    {
        private readonly IDataService _data; 
        private readonly Func<User, ProfilesPerUserListViewModel> _getProfilesPerUser;
        private readonly Func<ProfilesListViewModel> _getProfiles;

        public UserViewModel(Func<User, ProfilesPerUserListViewModel> getUserProfiles, Func<ProfilesListViewModel> getProfiles, IDataService data)
        {
            _getProfilesPerUser = getUserProfiles;
            _getProfiles = getProfiles;
            _data = data;
            H.Initialize(this);
        }


        public ProfilesPerUserListViewModel UserProfiles => _userProfiles.Get();
        private readonly IProperty<ProfilesPerUserListViewModel> _userProfiles = H.Property<ProfilesPerUserListViewModel>(c => c
            .On(e => e.Model)
            .NotNull(e => e.Model)
            .Set(e => e._getProfilesPerUser(e.Model))
        );


        public ProfilesListViewModel Profiles => _profiles.Get();
        private readonly IProperty<ProfilesListViewModel> _profiles = H.Property<ProfilesListViewModel>(c => c
            .On(e => e.Model)
            .Set(e => e._getProfiles())
        );

        public ICommand ChangePasswordCommand { get; } = H.Command(c => c
            .CanExecute(e => e.Model.Id == e.Acl.Connection.UserId)
        );

        public ICommand AddProfileCommand { get; } = H.Command(c => c
            .CanExecute(e => e.Acl.IsGranted(e.EditRight))
            .Action((e,p) => e.AddProfile(p as Profile) )
        );
        public ICommand RemoveProfileCommand { get; } = H.Command(c => c
            .CanExecute(e => e.UserProfiles.Selected != null)
            .Action((e,p) => e.RemoveProfile(e.UserProfiles.Selected) )
            .On(e => e.UserProfiles.Selected).CheckCanExecute()
        );

        private void RemoveProfile(UserProfile userProfile)
        {
            if (_data.Delete<UserProfile>(userProfile))
            {
                UserProfiles.List.Update();
            }
        }

        private void AddProfile(Profile profile)
        {
            if(profile==null) return;
            if(UserProfiles.List.Any(p => p.ProfileId==profile.Id)) return;

            var userProfile = _data.Add<UserProfile>(up =>
            {
                up.Profile = profile;
                up.User = Model;
            });
            if(userProfile!=null)
            {
                UserProfiles.List.Update();
            }
        }

        public override AclRight EditRight => AclRights.ManageUser;

        public bool EditMode => _editMode.Get();
        private readonly IProperty<bool> _editMode = H.Property<bool>(c => c
            .On(e => e.Locker.IsActive)
            .Set(e => e.Locker.IsActive)
        );


    }
}

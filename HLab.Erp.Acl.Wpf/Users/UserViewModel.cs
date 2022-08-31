using System;
using System.Linq;
using System.Windows.Input;
using HLab.Erp.Acl.Profiles;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl.Users
{
    using H = H<UserViewModel>;

    public class UserViewModel: ListableEntityViewModel<User>
    {
        public class Design : UserViewModel, IViewModelDesign
        {
            public Design()
            {
                Model = User.DesignModel;
            }
        }

        UserViewModel() {}


        readonly Func<User, ProfilesPerUserListViewModel> _getProfilesPerUser;
        readonly Func<ProfilesListViewModel> _getProfiles;

        public UserViewModel(
            Injector i,
            Func<User, ProfilesPerUserListViewModel> getUserProfiles,
            Func<ProfilesListViewModel> getProfiles
            ):base(i)
        {
            _getProfilesPerUser = getUserProfiles;
            _getProfiles = getProfiles;
            H.Initialize(this);
        }


        public ProfilesPerUserListViewModel UserProfiles => _userProfiles.Get();

        readonly IProperty<ProfilesPerUserListViewModel> _userProfiles = H.Property<ProfilesPerUserListViewModel>(c => c
            .On(e => e.Model)
            .NotNull(e => e.Model)
            .Set(e => e._getProfilesPerUser(e.Model))
        );


        public ProfilesListViewModel Profiles => _profiles.Get();

        readonly IProperty<ProfilesListViewModel> _profiles = H.Property<ProfilesListViewModel>(c => c
            .On(e => e.Model)
            .Set(e => e._getProfiles())
        );

        public ICommand ChangePasswordCommand { get; } = H.Command(c => c
            .CanExecute(e => e.Model.Id == e.Injected.Acl.Connection.UserId)
        );

        public ICommand AddProfileCommand { get; } = H.Command(c => c
            .CanExecute(e => e.Injected.Acl.IsGranted(e.EditRight))
            .Action((e,p) => e.AddProfile(p as Profile) )
        );
        public ICommand RemoveProfileCommand { get; } = H.Command(c => c
            .CanExecute(e => e.UserProfiles.Selected != null)
            .Action((e,p) => e.RemoveProfile(e.UserProfiles.Selected) )
            .On(e => e.UserProfiles.Selected).CheckCanExecute()
        );

        void RemoveProfile(UserProfile userProfile)
        {
            if (Injected.Data.Delete<UserProfile>(userProfile))
            {
                UserProfiles.List.Update();
            }
        }

        void AddProfile(Profile profile)
        {
            if(profile==null) return;
            if(UserProfiles.List.Any(p => p.ProfileId==profile.Id)) return;

            var userProfile = Injected.Data.Add<UserProfile>(up =>
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

        readonly IProperty<bool> _editMode = H.Property<bool>(c => c
            .On(e => e.Locker.IsActive)
            .Set(e => e.Locker.IsActive)
        );


    }
}

using System;
using System.Linq;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl.Profiles;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl.Users
{
    using H = H<UserViewModel>;

    public class UserViewModel: EntityViewModel<User>
    {
        public UserViewModel() => H.Initialize(this);

        public override string Title => _title.Get();
        public IProperty<string> _title = H.Property<string>( c=> c
            .Bind(e => e.Model.Name)
        );

        [Import]
        private readonly Func<User, ListUserProfileViewModel> _getUserProfiles;

        public ListUserProfileViewModel UserProfiles => _userProfiles.Get();
        private readonly IProperty<ListUserProfileViewModel> _userProfiles = H.Property<ListUserProfileViewModel>(c => c
            .On(e => e.Model)
            .NotNull(e => e.Model)
            .Set(e => e._getUserProfiles(e.Model))
        );

        [Import]
        private readonly Func<User, ProfilesListViewModel> _getProfiles;

        public ProfilesListViewModel Profiles => _profiles.Get();
        private readonly IProperty<ProfilesListViewModel> _profiles = H.Property<ProfilesListViewModel>(c => c
            .On(e => e.Model)
            .Set(e => e._getProfiles(e.Model))
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
        private IProperty<bool> _editMode = H.Property<bool>(c => c
            .On(e => e.Locker.IsActive)
            .Set(e => e.Locker.IsActive)
        );

        [Import]
        private IDataService _data; 

    }
}

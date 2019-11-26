using System;
using System.Linq;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Erp.Base.Wpf.Entities.Profiles;
using HLab.Erp.Data;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Wpf.Entities.Users
{
    public class UserViewModel: EntityViewModel<UserViewModel,User>
    {

        [Import]
        private IAclService _acl;

        public string Title => Model.Name;

        [Import]
        private readonly Func<User, ListUserProfileViewModel> _getUserProfiles;

        public ListUserProfileViewModel UserProfiles => _userProfiles.Get();
        private readonly IProperty<ListUserProfileViewModel> _userProfiles = H.Property<ListUserProfileViewModel>(c => c
            .On(e => e.Model)
            .NotNull(e => e.Model)
            .Set(e => e._getUserProfiles(e.Model))
        );

        [Import]
        private readonly Func<User, ListProfileViewModel> _getProfiles;

        public ListProfileViewModel Profiles => _profiles.Get();
        private readonly IProperty<ListProfileViewModel> _profiles = H.Property<ListProfileViewModel>(c => c
            .On(e => e.Model)
            .Set(e => e._getProfiles(e.Model))
        );

        public ICommand ChangePasswordCommand { get; } = H.Command(c => c
            .CanExecute(e => e.Model.Id == e._acl.Connection.UserId)
        );

        public ICommand AddProfileCommand { get; } = H.Command(c => c
            .CanExecute(e => e.Model.Id == e._acl.Connection.UserId)
            .Action((e,p) => e.AddProfile(p as Profile) )
        );
        public ICommand RemoveProfileCommand { get; } = H.Command(c => c
            .CanExecute(e => e.UserProfiles.Selected != null)
            .Action((e,p) => e.RemoveProfile(e.UserProfiles.Selected) )
            .On(e => e.UserProfiles.Selected).CheckCanExecute()
        );

        private void RemoveProfile(UserProfile userProfile)
        {
            var n = _data.Delete<UserProfile>(userProfile);
            if(n>0) UserProfiles.List.Update();
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

        [Import]
        private IDataService _data; 

    }
}

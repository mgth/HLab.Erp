using System;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
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

        private readonly Func<int, ListUserProfileViewModel> _getUserProfiles;

        public ListUserProfileViewModel UserProfiles => _userProfiles.Get();
        private readonly IProperty<ListUserProfileViewModel> _userProfiles = H.Property<ListUserProfileViewModel>(c => c
            .On(e => e.Model)
            .Set(e => e._getUserProfiles(e.Model.Id))
        );

        private readonly Func<int, ListProfileViewModel> _getProfiles;

        public ListProfileViewModel Profiles => _profiles.Get();
        private readonly IProperty<ListProfileViewModel> _profiles = H.Property<ListProfileViewModel>(c => c
            .On(e => e.Model)
            .Set(e => e._getProfiles(e.Model.Id))
        );

        public ICommand ChangePasswordCommand { get; } = H.Command(c => c
            .CanExecute(e => e.Model.Id == e._acl.Connection.UserId)
        );

        public ICommand AddProfileCommand { get; } = H.Command(c => c
            .CanExecute(e => e.Model.Id == e._acl.Connection.UserId)
            .Action(e => e._data.Add<UserProfile>(up =>
            {
                up.Profile = e.Profiles.Selected;
                up.User = e.Model;
            }))
        );

        [Import]
        private IDataService _data; 

        public ICommand RemoveProfileCommand { get; } = H.Command(c => c
            .CanExecute(e => e.Model.Id == e._acl.Connection.UserId)
            .Action(e => e._data.Delete(e.UserProfiles.Selected))
        );
    }
}

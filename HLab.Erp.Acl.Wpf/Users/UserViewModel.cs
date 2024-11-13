using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using HLab.Erp.Acl.Profiles;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using ReactiveUI;


namespace HLab.Erp.Acl.Users;

public class UserViewModel: ListableEntityViewModel<User>
{
    public class Design : UserViewModel, IDesignViewModel
    {
        public Design()
        {
            Model = User.DesignModel;
        }
    }

    UserViewModel() 
    {
        _userProfiles = this
            .WhenAnyValue(e => e.Model)
            .Select(m => _getProfilesPerUser(m))
            .ToProperty(this, e => e.UserProfiles);

        _profiles = this
            .WhenAnyValue(e => e.Model)
            .Select(m => _getProfiles())
            .ToProperty(this, e => e.Profiles);

        ChangePasswordCommand = ReactiveCommand.Create(()=>{ }, 
            this.WhenAnyValue(
                e => e.Model.Id,
                e => e.Injected.Acl.Connection.UserId,
                selector : (id, userId) => id == userId
            ));

        AddProfileCommand = ReactiveCommand.Create<Profile>(
            AddProfile, this
                .WhenAnyValue(e => e.Injected.Acl)
                .Select(acl => acl.IsGranted(EditRight)));

        RemoveProfileCommand = ReactiveCommand.Create<UserProfile>(
            RemoveProfile, this
                .WhenAnyValue(e => e.UserProfiles.Selected)
                .Select(p => p != null)
        );

        _editMode = this.WhenAnyValue(e => e.Locker.IsActive).ToProperty(this, e => e.EditMode);
    }


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
            
    }

    public ProfilesPerUserListViewModel UserProfiles => _userProfiles.Value;
    readonly ObservableAsPropertyHelper<ProfilesPerUserListViewModel> _userProfiles;

    public ProfilesListViewModel Profiles => _profiles.Value;
    readonly ObservableAsPropertyHelper<ProfilesListViewModel> _profiles;

    public ICommand ChangePasswordCommand { get; } 

    public ICommand AddProfileCommand { get; }
    public ICommand RemoveProfileCommand { get; }

    void RemoveProfile(UserProfile userProfile)
    {
        if (Injected.Data.Delete<UserProfile>(userProfile))
        {
            UserProfiles.List.Update();
        }
    }

    void AddProfile(Profile? profile)
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

    public override AclRight? EditRight => AclRights.ManageUser;

    public bool EditMode => _editMode.Value;
    readonly ObservableAsPropertyHelper<bool> _editMode;

}
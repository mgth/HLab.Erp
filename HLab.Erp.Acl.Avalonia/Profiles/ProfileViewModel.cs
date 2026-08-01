using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using HLab.Erp.Acl.Users;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Data;
using ReactiveUI;


namespace HLab.Erp.Acl.Profiles;

public class ProfileViewModel : ListableEntityViewModel<Profile>
{
    public ProfileViewModel(
        Injector i,
        IDataService data,
        Func<Profile, UsersPerProfileListViewModel> getUserProfiles, 
        Func<Profile, AclRightsProfileListViewModel> getRightProfiles) : base(i)
    {
        _getUsersPerProfile = getUserProfiles;
        _getRightProfiles = getRightProfiles;
        _data = data;
            
        _userProfiles = this
            .WhenAnyValue(e => e.Model)
            .Select(m => _getUsersPerProfile(m))
            .ToProperty(this, e => e.UserProfiles);

        _profileRights = this
            .WhenAnyValue(e => e.Model)
            .Select(m => _getRightProfiles(m))
            .ToProperty(this, e => e.ProfileRights);

        AddUserCommand = ReactiveCommand.Create<User>(AddUser);
        AddAclRightCommand = ReactiveCommand.Create<AclRight>(AddRight);
        RemoveUserCommand = ReactiveCommand.Create(
            ()=>RemoveUser(UserProfiles.Selected),
            this.WhenAnyValue(e => e.UserProfiles.Selected).Select(u => u != null)
        );
    }

    public class AclRightsProfileListViewModel : Core.EntityLists.EntityListViewModel<AclRightProfile>
    {
        public AclRightsProfileListViewModel(Injector i, Profile profile) : base(i, c => c
            .StaticFilter(e => e.ProfileId == profile.Id)
            .Column("Name")
            .Header("{Name}")
            .Content(s => s.AclRight.Caption)
        )
        {
            OpenAction = target => { };
        }

        protected override bool OpenCanExecute(AclRightProfile rightProfile, Action<string> errorAction) => false;
    }

    readonly Func<Profile, UsersPerProfileListViewModel> _getUsersPerProfile;

    public UsersPerProfileListViewModel UserProfiles => _userProfiles.Value;
    readonly ObservableAsPropertyHelper<UsersPerProfileListViewModel> _userProfiles;

    readonly Func<Profile, AclRightsProfileListViewModel> _getRightProfiles;
    public AclRightsProfileListViewModel ProfileRights => _profileRights.Value;
    readonly ObservableAsPropertyHelper<AclRightsProfileListViewModel> _profileRights;

    public ICommand AddUserCommand { get; } 
    public ICommand AddAclRightCommand { get; } 
    public ICommand RemoveUserCommand { get; } 

    public ICommand RemoveAclRightCommand { get; } 

    readonly IDataService _data;

    void AddUser(User? user)
    {
        if (user == null) return;
        if (UserProfiles.List.Any(p => p.UserId == user.Id)) return;

        var up = _data.Add<UserProfile>(u =>
        {
            u.Profile = Model;
            u.User = user;
        });
        if (up != null)
            UserProfiles.List.Update();
    }

    void AddRight(AclRight? right)
    {
        if (right == null) return;
        if (ProfileRights.List.Any(p => p.AclRightId == right.Id)) return;

        var up = _data.Add<AclRightProfile>(u =>
        {
            u.Profile = Model;
            u.AclRight = right;
        });
        if (up != null)
            ProfileRights.List.Update();
    }

    void RemoveUser(UserProfile userProfile)
    {
        if(userProfile==null) return;
        if(!UserProfiles.List.Contains(userProfile)) return;

        if(_data.Delete(userProfile))
        {
            UserProfiles.List.Update();
        }
    }

    void RemoveRight(AclRightProfile right)
    {
        if(right==null) return;
        if(!ProfileRights.List.Contains(right)) return;

        if(_data.Delete(right))
        {
            ProfileRights.List.Update();
        }
    }
}
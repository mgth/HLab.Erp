using System;
using System.Linq;
using System.Windows.Input;
using HLab.Erp.Acl.Users;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl.Profiles
{
    using H = H<ProfileViewModel>;

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
            H.Initialize(this);
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

            protected override bool CanExecuteOpen(AclRightProfile rightProfile, Action<string> errorAction) => false;
        }

        readonly Func<Profile, UsersPerProfileListViewModel> _getUsersPerProfile;

        public UsersPerProfileListViewModel UserProfiles => _userProfiles.Get();

        readonly IProperty<UsersPerProfileListViewModel> _userProfiles = H.Property<UsersPerProfileListViewModel>(c => c
            .NotNull(e => e.Model)
            .Set(e => e._getUsersPerProfile(e.Model))
            .On(e => e.Model)
            .Update()
        );

        readonly Func<Profile, AclRightsProfileListViewModel> _getRightProfiles;
        public AclRightsProfileListViewModel ProfileRights => _profileRights.Get();

        readonly IProperty<AclRightsProfileListViewModel> _profileRights = H.Property<AclRightsProfileListViewModel>(c => c
            .NotNull(e => e.Model)
            .Set(e => e._getRightProfiles(e.Model))            
            .On(e => e.Model)
            .Update()
        );

        public ICommand AddUserCommand { get; } = H.Command(c => c
            .Action((p, u) => p.AddUser(u as User))
        );
        public ICommand AddAclRightCommand { get; } = H.Command(c => c
            .Action((p, u) => p.AddRight(u as AclRight))
        );
        public ICommand RemoveUserCommand { get; } = H.Command(c => c
            .CanExecute(p => p.UserProfiles.Selected !=null)
            .Action((p) => p.RemoveUser(p.UserProfiles.Selected))
            .On(p => p.UserProfiles.Selected).CheckCanExecute()
        );

        public ICommand RemoveAclRightCommand { get; } = H.Command(c => c
            .CanExecute(p => p.ProfileRights.Selected != null && p.Injected.Acl.IsGranted(AclRights.ManageProfiles))
            .Action((p) => p.RemoveRight(p.ProfileRights.Selected))
            .On(p => p.ProfileRights.Selected).CheckCanExecute()
        );

        readonly IDataService _data;

        void AddUser(User user)
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

        void AddRight(AclRight right)
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
}

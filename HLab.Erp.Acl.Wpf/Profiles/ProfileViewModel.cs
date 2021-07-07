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

    public class ProfileViewModel : EntityViewModel<Profile>
    {
        public ProfileViewModel(
            IDataService data,
            Func<Profile, UsersPerProfileListViewModel> getUserProfiles, 
            Func<Profile, AclRightsProfileListViewModel> getRightProfiles)
        {
            _getUsersPerProfile = getUserProfiles;
            _getRightProfiles = getRightProfiles;
            _data = data;
            H.Initialize(this);
        }

        public class AclRightsProfileListViewModel : EntityListViewModel<AclRightProfile>
        {
            public AclRightsProfileListViewModel(Profile profile) : base(c => c
               .StaticFilter(e => e.ProfileId == profile.Id)
               .Column()
               .Header("{Name}")
               .Content(s => s.AclRight.Caption)
            )
            {
                OpenAction = target => { };
            }

            protected override bool CanExecuteOpen(AclRightProfile rightProfile, Action<string> errorAction) => false;
        }

        public override string Header => _header.Get();
        private readonly IProperty<string> _header = H.Property<string>(c => c
            .On(e => e.Model.Name).Set(e => "{Profile}\n"+e.Model.Name)
        );
        public override string IconPath => "Icons/Entities/Profile";

        private readonly Func<Profile, UsersPerProfileListViewModel> _getUsersPerProfile;

        public UsersPerProfileListViewModel UserProfiles => _userProfiles.Get();
        private readonly IProperty<UsersPerProfileListViewModel> _userProfiles = H.Property<UsersPerProfileListViewModel>(c => c
            .On(e => e.Model)
            .NotNull(e => e.Model)
            .Set(e => e._getUsersPerProfile(e.Model))
        );

        private readonly Func<Profile, AclRightsProfileListViewModel> _getRightProfiles;
        public AclRightsProfileListViewModel ProfileRights => _profileRights.Get();
        private readonly IProperty<AclRightsProfileListViewModel> _profileRights = H.Property<AclRightsProfileListViewModel>(c => c
            .On(e => e.Model)
            .NotNull(e => e.Model)
            .Set(e => e._getRightProfiles(e.Model))
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
            .CanExecute(p => p.ProfileRights.Selected != null && p.Acl.IsGranted(AclRights.ManageProfiles))
            .Action((p) => p.RemoveRight(p.ProfileRights.Selected))
            .On(p => p.ProfileRights.Selected).CheckCanExecute()
        );

        private readonly IDataService _data;
        private void AddUser(User user)
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
        private void AddRight(AclRight right)
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

        private void RemoveUser(UserProfile userProfile)
        {
            if(userProfile==null) return;
            if(!UserProfiles.List.Contains(userProfile)) return;

            if(_data.Delete(userProfile))
            {
                UserProfiles.List.Update();
            }
        }
        private void RemoveRight(AclRightProfile right)
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

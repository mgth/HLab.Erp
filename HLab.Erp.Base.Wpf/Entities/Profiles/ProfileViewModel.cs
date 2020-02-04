using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Base.Wpf.Entities.Users;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Wpf.Entities.Profiles
{
    public class ProfileViewModel : EntityViewModel<ProfileViewModel,Profile>
    {
        public string Title => _title.Get();
        private IProperty<string> _title = H.Property<string>(c => c
            .On(e => e.Model.Name).Set(e => "{Profile}\n"+e.Model.Name)
        );
        public string IconPath => "Icons/Entities/Profile";

        [Import]
        public IAclService Acl {get;}


        [Import]
        private readonly Func<Profile, ListUserProfileViewModel> _getUserProfiles;

        public ListUserProfileViewModel UserProfiles => _userProfiles.Get();
        private readonly IProperty<ListUserProfileViewModel> _userProfiles = H.Property<ListUserProfileViewModel>(c => c
            .On(e => e.Model)
            .NotNull(e => e.Model)
            .Set(e => e._getUserProfiles(e.Model))
        );

        [Import]
        private readonly Func<Profile, ListAclRightProfileViewModel> _getRightProfiles;
        public ListAclRightProfileViewModel ProfileRights => _profileRights.Get();
        private readonly IProperty<ListAclRightProfileViewModel> _profileRights = H.Property<ListAclRightProfileViewModel>(c => c
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

        [Import] private IDataService _data;
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
                UserProfiles.List.UpdateAsync();
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
                ProfileRights.List.UpdateAsync();
        }

        private void RemoveUser(UserProfile userProfile)
        {
            if(userProfile==null) return;
            if(!UserProfiles.List.Contains(userProfile)) return;

            if(_data.Delete<UserProfile>(userProfile)>0)
            {
                UserProfiles.List.UpdateAsync();
            }
        }
        private void RemoveRight(AclRightProfile right)
        {
            if(right==null) return;
            if(!ProfileRights.List.Contains(right)) return;

            if(_data.Delete<AclRightProfile>(right)>0)
            {
                ProfileRights.List.UpdateAsync();
            }
        }
    }
}

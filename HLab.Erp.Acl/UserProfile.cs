using HLab.Erp.Data;
using NPoco;
using ReactiveUI;

namespace HLab.Erp.Acl;

public class UserProfile : Entity
{
    public UserProfile() 
    {
        _profile = Foreign(this, e => e.ProfileId, e => e.Profile);
        _user = Foreign(this, e => e.UserId, e => e.User);
    }

    public int? ProfileId
    {
        get => _profileId;
        set => this.RaiseAndSetIfChanged(ref _profileId, value);
    }
    int? _profileId;

    [Ignore]
    public Profile Profile
    {
        get => _profile.Value;
        set => ProfileId = value.Id;
    }
    readonly ObservableAsPropertyHelper<Profile> _profile;

    public int? UserId
    {
        get => _userId;
        set =>this.RaiseAndSetIfChanged(ref _userId, value);
    }
    int? _userId;

    [Ignore]
    public User User
    {
        get => _user.Value;
        set => UserId = value.Id;
    }
    readonly ObservableAsPropertyHelper<User> _user;
}
using HLab.Erp.Data;
using HLab.Erp.Data.foreigners;
using NPoco;

namespace HLab.Erp.Acl;

public class UserProfile : Entity
{
    public UserProfile() 
    {
        _profile = this.Foreign( e => e.ProfileId, e => e.Profile);
        _user = this.Foreign( e => e.UserId, e => e.User);
    }

    public int? ProfileId
    {
        get => _profile.Id;
        set => _profile.SetId(value);
    }
    [Ignore]
    public Profile? Profile
    {
        get => _profile.Value;
        set => ProfileId = value.Id;
    }
    readonly ForeignPropertyHelper<UserProfile, Profile?> _profile;

    public int? UserId
    {
        get => _user.Id;
        set => _user.SetId(value);
    }
    [Ignore]
    public User? User
    {
        get => _user.Value;
        set => UserId = value.Id;
    }
    readonly ForeignPropertyHelper<UserProfile, User?> _user;
}
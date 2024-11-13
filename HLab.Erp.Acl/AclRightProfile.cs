using HLab.Erp.Data;
using HLab.Erp.Data.foreigners;
using NPoco;
using ReactiveUI;

namespace HLab.Erp.Acl;

public class AclRightProfile : Entity
{
    public AclRightProfile()
    {
        _profile = this.Foreign(e => e.ProfileId, e => e.Profile);
        _aclRight = this.Foreign(e => e.AclRightId, e => e.AclRight);
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
        set => ProfileId = value?.Id;
    }
    readonly ForeignPropertyHelper<AclRightProfile,Profile> _profile;

    public int? AclRightId
    {
        get => _aclRight.Id;
        set => _aclRight.SetId(value);
    }

    [Ignore]
    public AclRight? AclRight
    {
        get => _aclRight.Value;
        set => AclRightId = value?.Id;
    }
    readonly ForeignPropertyHelper<AclRightProfile, AclRight> _aclRight;
}

using HLab.Erp.Data;
using NPoco;
using ReactiveUI;

namespace HLab.Erp.Acl;

public class AclRightProfile : Entity
{
    public AclRightProfile()
    {

    }

    public int? ProfileId
    {
        get => _profileId;
        set => this.RaiseAndSetIfChanged(ref _profileId, value);
    }
    int? _profileId;

    [Ignore]
    public Profile? Profile
    {
        get => _profile.Value;
        set => ProfileId = value.Id;
    }
    readonly ObservableAsPropertyHelper<Profile?> _profile;

    public int? AclRightId
    {
        get => _aclRightId;
        set => this.RaiseAndSetIfChanged(ref _aclRightId, value);
    }
    int? _aclRightId;

    [Ignore]
    public AclRight AclRight
    {
        get => _aclRight.Value;
        set => AclRightId = value.Id;
    }
    readonly ObservableAsPropertyHelper<AclRight> _aclRight;
}

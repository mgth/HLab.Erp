namespace HLab.Erp.Acl;

public static class AclRights
{
    public static readonly AclRight? ManageUser = AclRight
        .Create("{Allow to create / Delete users}");

    public static readonly AclRight? ChangePassword = AclRight
        .Create("{Allow to change other users password}");

    public static readonly AclRight? ManageProfiles = AclRight
        .Create("{Allow to create / delete user profiles}");

    public static readonly AclRight? ManageRights = AclRight
        .Create("{Allow to add / remove rights for one profile}");

    public static readonly AclRight? BetaTest = AclRight
        .Create("{Allow users to test Beta features}");
}
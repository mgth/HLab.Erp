namespace HLab.Erp.Acl;

public static class AclRights
{
    /// <summary>
    /// Allow to create / Delete users
    /// </summary>        
    public static readonly AclRight ManageUser = AclRight.Get();
    /// <summary>
    /// Allow to change other users password
    /// </summary>        
    public static readonly AclRight ChangePassword = AclRight.Get();
    /// <summary>
    /// Allow to create / delete user profiles
    /// </summary>
    public static readonly AclRight ManageProfiles = AclRight.Get();
    /// <summary>
    /// Allow to add / remove rights for one profile
    /// </summary>
    public static readonly AclRight ManageRights = AclRight.Get();
    /// <summary>
    /// Allow users to test Beta features
    /// </summary>
    public static readonly AclRight BetaTest = AclRight.Get();
}
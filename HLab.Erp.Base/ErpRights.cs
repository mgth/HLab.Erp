using HLab.Erp.Acl;

namespace HLab.Erp.Base;

public static class ErpRights
{
    /// <summary>
    /// Allow to create / Delete users
    /// </summary>        
    public static readonly AclRight? ErpSignCustomer = AclRight.Create();
    public static readonly AclRight? ErpViewCustomer = AclRight.Create();
    public static readonly AclRight? ErpValidateCustomer = AclRight.Create();
    public static readonly AclRight? ErpManageCountries = AclRight.Create();
}
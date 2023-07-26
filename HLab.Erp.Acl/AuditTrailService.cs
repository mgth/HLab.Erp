namespace HLab.Erp.Acl;


public interface IAuditTrailProvider
{
    bool Audit(string action,AclRight rightNeeded, string log, object entity, string caption, string iconPath, bool sign, bool motivate);
}



using System.Threading.Tasks;

namespace HLab.Erp.Acl.AuditTrails;


public interface IAuditTrailProvider
{
   Task<bool> Audit(string action, AclRight rightNeeded, string log, object entity, string caption, string iconPath, bool sign, bool motivate);
}



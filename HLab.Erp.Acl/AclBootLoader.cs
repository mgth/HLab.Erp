using HLab.Core.Annotations;
using HLab.Erp.Data;
using System.Threading.Tasks;

namespace HLab.Erp.Acl;

public class AclBootLoader(IDataService? data) : Bootloader
{
   protected override BootState Load()
   {
      AclRight.Data = data;
      return BootState.Completed;
   }
}
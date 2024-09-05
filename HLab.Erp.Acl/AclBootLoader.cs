using HLab.Core.Annotations;
using HLab.Erp.Data;

namespace HLab.Erp.Acl;

public class AclBootLoader(IDataService data) : IBootloader
{
    public void Load(IBootContext bootstrapper)
    {
        AclRight.Data = data;
    }
}
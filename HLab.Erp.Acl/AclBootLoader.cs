using HLab.Core.Annotations;
using HLab.Erp.Data;
using System.Threading.Tasks;

namespace HLab.Erp.Acl;

public class AclBootLoader(IDataService? data) : IBootloader
{
    public Task LoadAsync(IBootContext bootstrapper)
    {
        AclRight.Data = data;
        return Task.CompletedTask;
    }
}
using HLab.Core.Annotations;
using HLab.Erp.Data;
using System.Threading.Tasks;

namespace HLab.Erp.Acl
{
    public class AclBootLoader(IDataService data) : IBootloader
    {
        readonly IDataService _data = data;

        public Task LoadAsync(IBootContext bootstrapper)
        {
            AclRight.Data = _data;
            return Task.CompletedTask;
        }
    }
}
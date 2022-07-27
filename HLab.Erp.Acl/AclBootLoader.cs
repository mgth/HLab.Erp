using HLab.Core.Annotations;
using HLab.Erp.Data;

namespace HLab.Erp.Acl
{
    public class AclBootLoader : IBootloader
    {
        readonly IDataService _data;

        public AclBootLoader(IDataService data)
        {
            _data = data;
        }

        public void Load(IBootContext bootstrapper)
        {
            AclRight.Data = _data;
        }
    }
}
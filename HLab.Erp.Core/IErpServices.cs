using HLab.Erp.Acl;
using HLab.Erp.Data;
using HLab.Mvvm.Application;

namespace HLab.Erp.Core
{
    public interface IErpServices : IApplicationService
    {
        IDataService Data { get; }
        IAclService Acl { get; }
    }
}
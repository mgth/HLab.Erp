using HLab.Erp.Acl;
using HLab.Erp.Workflows.Interfaces;

namespace HLab.Erp.Workflows.Extensions;

public interface IWorkFlowLockerViewModel
{
    public IDataLocker Locker{ get; }
    public IWorkflow Workflow { get; }
}
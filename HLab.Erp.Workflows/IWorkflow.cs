using System;
using System.Collections.ObjectModel;
using HLab.Erp.Acl;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Workflows
{
    public interface IWorkflow
    {
        User User { get; set; }
        string Caption { get; }
        string IconPath { get; }
        string SubIconPath { get; }
        ReadOnlyObservableCollection<WorkflowAction> Actions { get; }
        object Target { get; }
    }

    public interface IWorkflow<T> : IWorkflow
    where T:class,IWorkflow<T>
    {
        bool SetState(Func<Workflow<T>.State> setState);
        Workflow<T>.State CurrentState { get; set; }
    }
}
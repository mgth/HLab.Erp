using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using HLab.Erp.Acl;
using HLab.Erp.Workflows.Models;

namespace HLab.Erp.Workflows.Interfaces
{
    public interface IWorkflow
    {
        User User { get; set; }
        string Caption { get; }
        string IconPath { get; }
        string SubIconPath { get; }
        ReadOnlyObservableCollection<WorkflowAction> Actions { get; }
        ReadOnlyObservableCollection<string> Highlights { get; }
        object Target { get; }
    }

    public interface IWorkflow<T> : IWorkflow
    where T : class, IWorkflow<T>
    {
        Task<bool> SetStageAsync(Func<Workflow<T>.Stage> setState, string caption, string iconPath, bool sign, bool motivate);
        Workflow<T>.Stage CurrentStage { get; set; }
    }
}
using System;
using System.Collections.Generic;
using HLab.Base.Fluent;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Workflows
{
    public interface IWorkflowConditional
    {
        WorkflowConditionResult Check(IWorkflow workflow);
        void Action(IWorkflow workflow);
        string GetCaption(IWorkflow workflow);
        string GetIcon(IWorkflow workflow);
        IEnumerable<string> GetMessages(IWorkflow workflow);
    }

    public interface IWorkflowAction : IWorkflowConditional
    {
    }

    public class WorkflowAction
    {
        private readonly IWorkflow _workflow;
        private readonly IWorkflowAction _action;
        public WorkflowAction(IWorkflow workflow, IWorkflowAction action)
        {
            _workflow = workflow;
            _action = action;
        }

        public string Caption => _action.GetCaption(_workflow);
        public string Icon => _action.GetIcon(_workflow);
        public void Action() => _action.Action(_workflow);
        public WorkflowConditionResult Check() => _action.Check(_workflow);
        public IEnumerable<string> Messages => _action.GetMessages(_workflow);

    }

    public enum WorkflowDirection
    {
        Backward,
        Forward,
    }

    public class WorkflowActionClass<T> : WorkflowConditionalObject<T> , IWorkflowAction
        where T : NotifierBase, IWorkflow<T>
    {
        public static WorkflowActionClass<T> Create(Action<IFluentConfigurator<WorkflowActionClass<T>>> configure)
        {
            var wfAction = new WorkflowActionClass<T>();
            var configurator = new FluentConfigurator<WorkflowActionClass<T>>(wfAction);
            configure(configurator);
            Workflow<T>.AddAction(wfAction);
            return wfAction;
        }

        public WorkflowDirection Direction { get; set; }

        public WorkflowAction GetAction(T workflow) => new WorkflowAction(workflow,this);
    }
}

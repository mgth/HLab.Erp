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
        string GetIconPath(IWorkflow workflow);
        IEnumerable<string> GetMessages(IWorkflow workflow);
        IEnumerable<string> GetHighlights(IWorkflow workflow);
    }

    public interface IWorkflowAction : IWorkflowConditional
    {
        WorkflowDirection Direction { get; }
        bool SigningMandatory {get;}
        bool MotivationMandatory {get;}
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
        public string IconPath => _action.GetIconPath(_workflow);
        public void Action() => _action.Action(_workflow);
        public WorkflowConditionResult Check() => _action.Check(_workflow);
        public IEnumerable<string> Messages => _action.GetMessages(_workflow);
        public IEnumerable<string> Highlights => _action.GetHighlights(_workflow);
        public WorkflowDirection Direction => _action.Direction;

        public bool SigningMandatory  => _action.SigningMandatory;
        public bool MotivationMandatory => _action.MotivationMandatory;

    }

    public enum WorkflowDirection
    {
        Forward,
        Backward,
    }

}

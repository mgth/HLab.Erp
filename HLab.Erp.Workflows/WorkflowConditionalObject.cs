﻿using System;
using System.Collections.Generic;
using HLab.Base.Fluent;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Workflows
{

    public interface IWorkflowConditionalObject<T>
        where T : NotifierBase, IWorkflow<T>
    {
        void AddCondition(WorkflowCondition<T> condition);
        void SetCaption(Func<T, string> getCaption);
        void SetIcon(Func<T, string> getIcon);
        void SetAction(Action<T> action);
        WorkflowCondition<T> Condition { get; }
    }

    public abstract class WorkflowConditionalObject<T> : IWorkflowConditionalObject<T>
        where T : NotifierBase, IWorkflow<T>
    {

        private Action<T> _action;
        public void Action(IWorkflow workflow) => _action?.Invoke((T)workflow);
        void IWorkflowConditionalObject<T>.SetAction(Action<T> action) => _action = action;


        public WorkflowCondition<T> Condition { get; private set; } = null;

        WorkflowCondition<T> IWorkflowConditionalObject<T>.Condition => Condition;

        void IWorkflowConditionalObject<T>.AddCondition(WorkflowCondition<T> condition)
        {
            condition.SetNext(Condition);
            Condition = condition;
        }

        private Func<T,string> _getIcon;
        void IWorkflowConditionalObject<T>.SetIcon(Func<T, string> getIcon) => _getIcon = getIcon;
        public string GetIcon(IWorkflow workflow) => _getIcon?.Invoke((T)workflow) ?? "";


        private Func<T,string> _getCaption;
        void IWorkflowConditionalObject<T>.SetCaption(Func<T, string> getCaption) => _getCaption = getCaption;
        public string GetCaption(IWorkflow workflow) => _getCaption?.Invoke((T)workflow) ?? "";


        public WorkflowConditionResult Check(IWorkflow workflow)
        {
            if (workflow is T wf)
            {
                return Condition?.CheckAll(wf)??WorkflowConditionResult.Passed;
            }
            else throw new Exception("Work workflow type");
        }

        public IEnumerable<string> GetMessages(IWorkflow workflow)
        {
            if (workflow is T wf)
            {
                foreach (var m in Condition.GetMessage(wf))
                {
                    yield return m;
                }
            }
        }

    }
}
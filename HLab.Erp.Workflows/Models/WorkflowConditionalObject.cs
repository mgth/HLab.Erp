﻿using HLab.Erp.Workflows.Interfaces;
using System;
using System.Collections.Generic;

namespace HLab.Erp.Workflows.Models
{
    public interface IWorkflowConditionalObject
    {
        string GetIconPath(IWorkflow workflow);
        string GetSubIconPath(IWorkflow workflow);
        string GetCaption(IWorkflow workflow);
        double GetProgress(IWorkflow workflow);
    }

    public interface IWorkflowConditionalObject<T> : IWorkflowConditionalObject
        where T : class, IWorkflow<T>
    {
        void AddCondition(WorkflowCondition<T> condition);
        void SetCaption(Func<T, string> getCaption);
        void SetIconPath(Func<T, string> getIcon);
        void SetSubIconPath(Func<T, string> getIcon);
        void SetAction(Action<T> action);
        void SetProgress(Func<T, double> getProgress);
        WorkflowCondition<T> Condition { get; }
    }

    public abstract class WorkflowConditionalObject<T> : IWorkflowConditionalObject<T>
        where T : class, IWorkflow<T>
    {
        Action<T> _action = null;
        public void Action(IWorkflow workflow) => _action?.Invoke((T)workflow);
        void IWorkflowConditionalObject<T>.SetAction(Action<T> action) => _action += action;

        public bool HasAction => _action != null;

        public WorkflowCondition<T> Condition { get; private set; } = null;

        WorkflowCondition<T> IWorkflowConditionalObject<T>.Condition => Condition;

        void IWorkflowConditionalObject<T>.AddCondition(WorkflowCondition<T> condition)
        {
            condition.SetNext(Condition);
            Condition = condition;
        }

        Func<T, string> _getIconPath;
        void IWorkflowConditionalObject<T>.SetIconPath(Func<T, string> getIcon) => _getIconPath = getIcon;
        public string GetIconPath(IWorkflow workflow) => _getIconPath?.Invoke((T)workflow) ?? "";

        Func<T, string> _getSubIconPath;
        void IWorkflowConditionalObject<T>.SetSubIconPath(Func<T, string> getIcon) => _getSubIconPath = getIcon;
        public string GetSubIconPath(IWorkflow workflow) => _getSubIconPath?.Invoke((T)workflow) ?? "";


        Func<T, string> _getCaption;
        void IWorkflowConditionalObject<T>.SetCaption(Func<T, string> getCaption) => _getCaption = getCaption;
        public string GetCaption(IWorkflow workflow) => _getCaption?.Invoke((T)workflow) ?? "";

        Func<T, double> _getProgress;
        void IWorkflowConditionalObject<T>.SetProgress(Func<T, double> getProgress) => _getProgress = getProgress;
        public double GetProgress(IWorkflow workflow) => _getProgress?.Invoke((T)workflow) ?? 0.0;


        public WorkflowConditionResult Check(IWorkflow workflow)
        {
            if (workflow is T wf)
            {
                return Condition?.CheckAll(wf) ?? WorkflowConditionResult.Passed;
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
        public IEnumerable<string> GetHighlights(IWorkflow workflow)
        {
            if (workflow is T wf)
            {
                foreach (var m in Condition.GetHighlights(wf))
                {
                    yield return m;
                }
            }
        }

    }
}

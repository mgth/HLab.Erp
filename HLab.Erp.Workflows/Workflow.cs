
using HLab.Erp.Acl;
using HLab.Mvvm.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Workflows
{
    public interface IWorkflow
    {
        User User { get; set; }
        string Caption { get; }
        string Icon { get; }
        ObservableCollection<WorkflowAction> Actions { get; }
    }

    public class Workflow : NotifierBase
    { }

    public interface IWorkflow<T> : IWorkflow
        where T : NotifierBase, IWorkflow<T>
    {
        bool SetState(Func<WorkflowState<T>> setState);
        WorkflowState<T> State { get; }
    }

    public abstract class Workflow<T> : Workflow, IWorkflow<T>
        where T : NotifierBase, IWorkflow<T>
    {
        protected class H : NotifyHelper<T> { }

        public User User { get; set; }

        private static readonly List<WorkflowState<T>> WorkflowStates = new List<WorkflowState<T>>();
        private static readonly List<WorkflowActionClass<T>> WorkflowActions = new List<WorkflowActionClass<T>>();
        public string Caption => _caption.Get();
        private readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.State)
            .Set(e => e.State.GetCaption(e))
        );
        public string Icon => _icon.Get();
        private readonly IProperty<string> _icon = H.Property<string>(c => c
            .On(e => e.State)
            .Set(e => e.State.GetIcon(e))
        );


        internal static void AddState(WorkflowState<T> state)
        {
            WorkflowStates.Add(state);
        }
        public static void AddAction(WorkflowActionClass<T> state)
        {
            WorkflowActions.Add(state);
        }


        public WorkflowState<T> State
        {
            get => _state.Get();
            protected set
            {
                value.Action(this as T);
                _state.Set(value);
            }
        }
        private readonly IProperty<WorkflowState<T>> _state = H.Property<WorkflowState<T>>();

        public bool SetState(Func<WorkflowState<T>> setState)
        {
            var state = setState();

            if (state.Check(this as T) == WorkflowConditionResult.Passed)
            {
                State = state;
                Update();
                return true;
            }

            return false;
        }

        public ObservableCollection<WorkflowAction> Actions { get; } = new ObservableCollection<WorkflowAction>();

        protected void Update()
        {
            Actions.Clear();
            var actions = WorkflowActions
                .Where(a => a.Check(this as T) != WorkflowConditionResult.Hidden)
                .Select(e => e.GetAction(this as T) );

            foreach (var action in actions)
            {
                Actions.Add(action);
            }
        }
    }

    public class Workflow<T, TTarget> : Workflow<T> where T : Workflow<T, TTarget>
    {
        public Workflow(TTarget target)
        {
            Target = target;
        }

        public TTarget Target { get; }
    }
}

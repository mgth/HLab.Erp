
using HLab.Erp.Acl;
using HLab.Mvvm.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using HLab.Base.Fluent;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Workflows
{
    public static class NotifyHelperExtension
    {

    }


    public interface IWorkflow
    {
        User User { get; set; }
        string Caption { get; }
        string Icon { get; }
        ObservableCollection<WorkflowAction> ForwardActions { get; }
        ObservableCollection<WorkflowAction> BackwardActions { get; }
    }

    public class Workflow : NotifierBase
    { }

    public interface IWorkflow<T> : IWorkflow
        where T : NotifierBase, IWorkflow<T>
    {
        bool SetState(Func<Workflow<T>.State> setState);
        Workflow<T>.State CurrentState { get; }
    }

    public abstract class Workflow<T> : Workflow, IWorkflow<T>
        where T : NotifierBase, IWorkflow<T>
    {
        protected class H : NotifyHelper<T> { }
        public class State : WorkflowConditionalObject<T> 
        {
            public static State Create(
                Action<IFluentConfigurator<State>> configure, 
                [CallerMemberName]string name="")
            {
                var state =  new State(name);
                var configurator = new FluentConfigurator<State>(state);
                configure?.Invoke(configurator);
                AddState(state);
                return state;
            }

            public string Name { get; }

            protected State(string name)
            {
                Name = name;
            }
        }

        public class Action : WorkflowConditionalObject<T> , IWorkflowAction
        {
            public static Action Create(Action<IFluentConfigurator<Action>> configure)
            {
                var action = new Action();
                var configurator = new FluentConfigurator<Action>(action);
                configure?.Invoke(configurator);
                AddAction(action);
                return action;
            }

            public WorkflowDirection Direction { get; set; }

            public WorkflowAction GetAction(T workflow) => new WorkflowAction(workflow,this);
        }

        public User User { get; set; }

        private static readonly List<State> WorkflowStates = new List<State>();
        private static readonly List<Action> WorkflowActions = new List<Action>();
        public string Caption => _caption.Get();
        private readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.CurrentState)
            .Set(e => e.CurrentState.GetCaption(e))
        );

        public string Icon => _icon.Get();

        private readonly IProperty<string> _icon = H.Property<string>(c => c
            .On(e => e.CurrentState)
            .Set(e => e.CurrentState.GetIcon(e))
        );


        internal static void AddState(State state)
        {
            WorkflowStates.Add(state);
        }
        public static void AddAction(Action state)
        {
            WorkflowActions.Add(state);
        }


        public State CurrentState
        {
            get => _state.Get();
            protected set
            {
                if (_state.Set(value))
                    value?.Action(this as T);
            }
        }
        private readonly IProperty<State> _state = H.Property<State>();

        public bool SetState(Func<State> setState)
        {
            if (setState == null) return false;

            var state = setState();

            if (state.Check(this as T) == WorkflowConditionResult.Passed)
            {
                CurrentState = state;
                OnSetState(state);
                Update();
                return true;
            }

            return false;
        }

        public virtual void OnSetState(State state)
        { }

        public ObservableCollection<WorkflowAction> ForwardActions { get; } = new ObservableCollection<WorkflowAction>();
        public ObservableCollection<WorkflowAction> BackwardActions { get; } = new ObservableCollection<WorkflowAction>();

        protected void Update()
        {
            ForwardActions.Clear();
            BackwardActions.Clear();
            var actions = WorkflowActions
                .Where(a => a.Check(this as T) != WorkflowConditionResult.Hidden)
                .Select(e => e.GetAction(this as T) );

            foreach (var action in actions)
            {
                if(action.Direction == WorkflowDirection.Backward)
                    BackwardActions.Add(action);
                else
                {
                    ForwardActions.Add(action);
                }
            }
        }
    }

    public class Workflow<T, TTarget> : Workflow<T> where T : Workflow<T, TTarget>
    where TTarget : INotifyPropertyChanged
    {
        public Workflow(TTarget target)
        {
            Target = target;
            Target.PropertyChanged += Target_PropertyChanged;
        }

        private void Target_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Update();
        }

        public TTarget Target { get; }
    }
}

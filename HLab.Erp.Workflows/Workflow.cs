
using HLab.Erp.Acl;
using HLab.Mvvm.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using HLab.Base.Fluent;
using HLab.DependencyInjection.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Workflows
{
    public static class NotifyHelperExtension
    {

    }



    public abstract class Workflow<T> : N<T>, IWorkflow<T>
        where T : /*Workflow<T>, */class, IWorkflow<T>
    {
        protected Workflow():base()
        {
            this.PropertyChanged += Workflow_PropertyChanged;
        }

        private void Workflow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName=="CurrentState")
            { }
        }

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
        public object Target { get; protected set; }

        private static readonly List<State> WorkflowStates = new List<State>();
        private static readonly List<Action> WorkflowActions = new List<Action>();

        public string Caption => _caption.Get();
        private readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.CurrentState)
            .Set(e => e.CurrentState.GetCaption(e))
            .Set(e => e.CurrentState.GetCaption(e))
        );

        public string IconPath => _iconPath.Get();
        private readonly IProperty<string> _iconPath = H.Property<string>(c => c
            .On(e => e.CurrentState)
            .Set(e => e.CurrentState.GetIconPath(e))
            .Set(e => e.CurrentState.GetIconPath(e))
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
            get => _currentState.Get();
            set
            {
                if (_currentState.Set(value))
                    value?.Action(this as T);
            }
        }
        private readonly IProperty<State> _currentState = H.Property<State>();

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

    public class Workflow<T, TTarget> : Workflow<T> 
        where T : Workflow<T, TTarget>
        where TTarget : INotifyPropertyChanged
    {
        public Workflow(TTarget target)
        {
            base.Target = target;
            Target.PropertyChanged += Target_PropertyChanged;
        }

        private void Target_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Update();
        }

        public new TTarget Target => (TTarget)base.Target;
    }
}

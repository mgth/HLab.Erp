
using HLab.Base.Fluent;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Notify.PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace HLab.Erp.Workflows
{
    public static class NotifyHelperExtension
    {

    }



    public abstract class Workflow<T> : N<T>, IWorkflow<T>
        where T : /*Workflow<T>, */class, IWorkflow<T>
    {
        protected Workflow(object target, IDataLocker locker)
        {
            Target = target;
            Locker = locker;
        }

        public class State : WorkflowConditionalObject<T> 
        {
            public static State CreateDefault(
                Action<IFluentConfigurator<State>> configure, 
                [CallerMemberName]string name="")
            {
                var state =  new State(name);
                var configurator = new FluentConfigurator<State>(state);
                configure?.Invoke(configurator);
                AddDefaultState(state);
                return state;
            }

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
        public object Target { get; }
        protected IDataLocker Locker { get; }

        private readonly IProperty<object> _locker = H.Property<object>();

        protected static readonly List<State> WorkflowStates = new List<State>();
        protected static readonly List<Action> WorkflowActions = new List<Action>();

        public string Caption => _caption.Get();
        private readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.CurrentState)
            .Set(e => e.CurrentState.GetCaption(e))
            //.NotNull(e =>e.CurrentState)
            .Set(e => e.CurrentState?.GetCaption(e))
        );

        public string IconPath => _iconPath.Get();
        private readonly IProperty<string> _iconPath = H.Property<string>(c => c
            .On(e => e.CurrentState)
            .Set(e => e.CurrentState.GetIconPath(e))
            //.NotNull(e =>e.CurrentState)
            .Set(e => e.CurrentState?.GetIconPath(e))
        );

        public string SubIconPath => _subIconPath.Get();
        private readonly IProperty<string> _subIconPath = H.Property<string>(c => c
            .On(e => e.CurrentState)
            .Set(e => e.CurrentState.GetSubIconPath(e))
            //.NotNull(e =>e.CurrentState)
            .Set(e => e.CurrentState?.GetSubIconPath(e))
        );

        internal static void AddState(State state)
        {
            WorkflowStates.Add(state);
        }
        internal static void AddDefaultState(State state)
        {
            WorkflowStates.Add(state);
            DefaultState = state;
        }

        public static void AddAction(Action state)
        {
            WorkflowActions.Add(state);
        }

        public static State DefaultState { get; private set; }

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
                if (OnSetState(state))
                {
                    CurrentState = state;
                    Update();
                    return true;
                }
            }

            return false;
        }

        protected abstract string StateName { get; set; }

        protected virtual bool OnSetState(State state)
        {
            if (StateName != state.Name)
            {
                var old = StateName;
                StateName = state.Name;
                Locker.SaveCommand.Execute(null);
                if (Locker.IsActive)
                {
                    StateName = old;
                    return false;
                }
                return true;
            }
            return true;
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

    public abstract class Workflow<T, TTarget> : Workflow<T> 
        where T : Workflow<T, TTarget>
        where TTarget : INotifyPropertyChanged
    {
        public Workflow(TTarget target, IDataLocker locker):base(target,locker)
        {
            Target.PropertyChanged += Target_PropertyChanged;
        }

        private void Target_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Update();
        }
        protected void SetState(string state) => CurrentState = WorkflowStates.Find(e => e.Name == state)??DefaultState;

        public new TTarget Target => (TTarget)base.Target;
    }
}

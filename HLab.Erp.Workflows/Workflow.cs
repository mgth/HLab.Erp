
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

            if(target is INotifyPropertyChanged n)
                n.PropertyChanged += Target_PropertyChanged;
        }

        private void Target_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Update();
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

        private static List<State> _workflowState;
        private static List<Action> _workflowAction;
        private static List<State> WorkflowStates => _workflowState??(_workflowState=new List<State>());
        private static List<Action> WorkflowActions => _workflowAction??(_workflowAction=new List<Action>());
        
        public static State StateFromName(string name) => WorkflowStates.Find(e => e.Name == name)??DefaultState;
        protected void SetState(string state) => CurrentState = StateFromName(state);


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

        internal static bool AddState(State state)
        {
            if (!WorkflowStates.Contains(state))
            {
                WorkflowStates.Add(state);
                return true;
            }
            return false;
        }
        internal static bool AddDefaultState(State state)
        {
            if (DefaultState != null) throw new Exception("Default state declared twice");
            if (AddState(state))
            {
                DefaultState = state;
                return true;
            }
            return false;
        }

        public static void AddAction(Action action)
        {
            if(!WorkflowActions.Contains(action))
                WorkflowActions.Add(action);
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


        private object _lock = new object();
        protected void Update()
        {
            lock(_lock)
            {
                Actions.Clear();
                WorkflowActions
                    .Where(a => a.Check(this as T) != WorkflowConditionResult.Hidden)
                    .ToList().ForEach(e => Actions.Add(e.GetAction(this as T)) );
            }
        }
    }

    public abstract class Workflow<T, TTarget> : Workflow<T> 
        where T : Workflow<T, TTarget>
        where TTarget : INotifyPropertyChanged
    {
        public Workflow(TTarget target, IDataLocker locker):base(target,locker)
        {
        }

        public new TTarget Target => (TTarget)base.Target;

    }
}

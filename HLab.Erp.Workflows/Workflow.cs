
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
using System.Threading;
using System.Threading.Tasks;

namespace HLab.Erp.Workflows
{
    public static class NotifyHelperExtension
    {

    }



    public abstract class Workflow<T> : NotifierBase, IWorkflow<T>
        where T : /*Workflow<T>, */class, IWorkflow<T>
    {
        protected Workflow(object target, IDataLocker locker)
        {
            Actions = new ReadOnlyObservableCollection<WorkflowAction>(_actions);
            Highlights = new ReadOnlyObservableCollection<string>(_highlights);

            H<Workflow<T>>.Initialize(this);

            Target = target;
            _locker.Set(locker);

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

                //if(!state.HasAction)
                {
                    var c = new Action<IFluentConfigurator<State>>(c => c
                        //.WhenStateAllowed(() => state)
                        .Action(async w => await w.SetStateAsync(() => state)));
                    c(new FluentConfigurator<State>(state));
                }

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

        //Todo : user never set
        public User User { get; set; }
        public object Target { get; }
        public IDataLocker Locker => _locker.Get();
        private readonly IProperty<IDataLocker> _locker = H<Workflow<T>>.Property<IDataLocker>();

        //private readonly IProperty<object> _locker = H.Property<object>();

        private static List<State> _workflowState;
        private static List<Action> _workflowAction;
        private static List<State> WorkflowStates => _workflowState ??= new List<State>();
        private static List<Action> WorkflowActions => _workflowAction ??= new List<Action>();
        
        public static State StateFromName(string name) => WorkflowStates.Find(e => e.Name == name)??DefaultState;
        protected void SetState(string state) => CurrentState = StateFromName(state);


        public string Caption => _caption.Get();
        private readonly IProperty<string> _caption = H<Workflow<T>>.Property<string>(c => c
            .Set(e => e.CurrentState?.GetCaption(e))
            .On(e => e.CurrentState)
            .Update()
        );

        public string IconPath => _iconPath.Get();
        private readonly IProperty<string> _iconPath = H<Workflow<T>>.Property<string>(c => c
            .Set(e => e.CurrentState?.GetIconPath(e))
            .On(e => e.CurrentState)
            .Update()
        );

        public string SubIconPath => _subIconPath.Get();
        private readonly IProperty<string> _subIconPath = H<Workflow<T>>.Property<string>(c => c
            .Set(e => e.CurrentState?.GetSubIconPath(e))
            .On(e => e.CurrentState)
            .Update()
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
        private readonly IProperty<State> _currentState = H<Workflow<T>>.Property<State>();

        public async Task<bool> SetStateAsync(Func<State> setState)
        {
            if (setState == null) return false;

            var state = setState();

            if (state.Check(this as T) == WorkflowConditionResult.Passed)
            {
                if (await OnSetStateAsync(state))
                {
                    CurrentState = state;
                    Update();
                    return true;
                }
            }

            return false;
        }

        protected abstract string StateName { get; set; }

        protected virtual async Task<bool> OnSetStateAsync(State state)
        {
            if (StateName != state.Name)
            {
                var old = StateName;
                StateName = state.Name;

                if (await Locker.SaveAsync()) return true;

                StateName = old;
                return false;
            }
            return true;
        }


        private readonly ObservableCollection<WorkflowAction> _actions = new ObservableCollection<WorkflowAction>();
        private readonly ObservableCollection<string> _highlights = new ObservableCollection<string>();
        public ReadOnlyObservableCollection<WorkflowAction> Actions { get; }
        public ReadOnlyObservableCollection<string> Highlights { get; }


        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        protected void Update()
        {
            var list = WorkflowActions
                .Where(a => a.Check(this as T) != WorkflowConditionResult.Hidden)
                .ToList();

            _lock.EnterWriteLock();
            try
            {
                _actions.Clear();
                _highlights.Clear();
                foreach (var action in list)
                {
                    var a = action.GetAction(this as T);
                    _actions.Add(a);
                    foreach(var h in a.Highlights) _highlights.Add(h);
                }

            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }

    public abstract class Workflow<T, TTarget> : Workflow<T> 
        where T : Workflow<T, TTarget>
        where TTarget : INotifyPropertyChanged
    {
        protected Workflow(TTarget target, IDataLocker locker):base(target,locker)
        {
        }

        public new TTarget Target => (TTarget)base.Target;

    }
}


using HLab.Base.Fluent;
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
    public interface IWorkflowStage : IWorkflowConditionalObject
    {
        string Name { get; }
    }

    public abstract class Workflow<T> : NotifierBase, IWorkflow<T>
        where T : /*Workflow<T>, */class, IWorkflow<T>
    {
        protected Workflow(object target, IDataLocker locker)
        {
            Actions = new(_actions);
            Highlights = new(_highlights);

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


        public class Stage : WorkflowConditionalObject<T> , IWorkflowStage
        {
            public static Stage CreateDefault(
                Action<IFluentConfigurator<Stage>> configure, 
                [CallerMemberName]string name="")
            {
                var stage =  new Stage(name);
                var configurator = new FluentConfigurator<Stage>(stage);
                configure?.Invoke(configurator);
                AddDefaultStage(stage);
                return stage;
            }

            public static Stage Create(
                Action<IFluentConfigurator<Stage>> configure, 
                [CallerMemberName]string name="")
            {
                var stage =  new Stage(name);
                var configurator = new FluentConfigurator<Stage>(stage);

                configure?.Invoke(configurator);

                //if(!stage.HasAction)
                {
                    var c = new Action<IFluentConfigurator<Stage>>(c => c
                        //.WhenStageAllowed(() => stage)
                        .Action(async w => await w.SetStageAsync(() => stage,"","", false,false)));
                    c(new FluentConfigurator<Stage>(stage));
                }

                AddStage(stage);
                return stage;
            }

            public string Name { get; }

            protected Stage(string name)
            {
                Name = name;
            }
            public bool IsAny(Action<string> errorAction, params Stage[] stages)
            {
                if (stages.Contains(this)) return true;
                foreach (var stage in stages)
                {
                    errorAction($"{{Stage needed}} : {{{stage.Name}}}");
                }

                return false;
            }

            public override string ToString() => Name;
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

            public bool SigningMandatory { get; set; }

            public bool MotivationMandatory { get; set; }

            public WorkflowAction GetAction(T workflow) => new WorkflowAction(workflow,this);
            public override string ToString() => GetAction(null).Caption;
        }

        //Todo : user never set
        //public User User { get; set; }
        public object Target { get; }
        public IDataLocker Locker => _locker.Get();
        private readonly IProperty<IDataLocker> _locker = H<Workflow<T>>.Property<IDataLocker>();

        //private readonly IProperty<object> _locker = H.Property<object>();

        private static List<Stage> _workflowStage;
        private static List<Action> _workflowAction;
        private static List<Stage> WorkflowStages => _workflowStage ??= new List<Stage>();
        private static List<Action> WorkflowActions => _workflowAction ??= new List<Action>();
        
        public static Stage StageFromName(string name) => WorkflowStages.Find(e => e.Name == name)??DefaultStage;
        protected void SetStage(Stage stage) => CurrentStage = stage;


        public string Caption => _caption.Get();
        private readonly IProperty<string> _caption = H<Workflow<T>>.Property<string>(c => c
            .Set(e => e.CurrentStage?.GetCaption(e))
            .On(e => e.CurrentStage)
            .Update()
        );

        public string IconPath => _iconPath.Get();
        private readonly IProperty<string> _iconPath = H<Workflow<T>>.Property<string>(c => c
            .Set(e => e.CurrentStage?.GetIconPath(e))
            .On(e => e.CurrentStage)
            .Update()
        );

        public string SubIconPath => _subIconPath.Get();
        private readonly IProperty<string> _subIconPath = H<Workflow<T>>.Property<string>(c => c
            .Set(e => e.CurrentStage?.GetSubIconPath(e))
            .On(e => e.CurrentStage)
            .Update()
        );

        internal static bool AddStage(Stage stage)
        {
            if (!WorkflowStages.Contains(stage))
            {
                WorkflowStages.Add(stage);
                return true;
            }
            return false;
        }
        internal static bool AddDefaultStage(Stage stage)
        {
            if (DefaultStage != null) throw new Exception("Default stage declared twice");
            if (AddStage(stage))
            {
                DefaultStage = stage;
                return true;
            }
            return false;
        }

        public static void AddAction(Action action)
        {
            if(!WorkflowActions.Contains(action))
                WorkflowActions.Add(action);
        }

        public static Stage DefaultStage { get; private set; }

        public Stage CurrentStage
        {
            get => _currentStage.Get();
            set
            {
                if (_currentStage.Set(value))
                    value?.Action(this as T);
            }
        }
        private readonly IProperty<Stage> _currentStage = H<Workflow<T>>.Property<Stage>();

        public async Task<bool> SetStageAsync(Func<Stage> setStage, string caption, string iconPath, bool sign, bool motivate)
        {
            if (setStage == null) return false;

            var stage = setStage();

            if (stage.Check(this as T) == WorkflowConditionResult.Passed)
            {
                if (await OnSetStageAsync(stage, caption, iconPath, sign, motivate))
                {
                    CurrentStage = stage;
                    Update();
                    return true;
                }
            }

            return false;
        }

        protected abstract Stage TargetStage { get; set; }
        protected virtual Stage TargetPreviousStage { get; set; }

        protected virtual async Task<bool> OnSetStageAsync(Stage stage, string caption, string iconPath, bool sign, bool motivate)
        {
            if (TargetStage == stage) return true;

            var old = TargetStage;
            TargetStage = stage;

            if (await Locker.SaveAsync(caption, iconPath, sign, motivate)) return true;

            TargetStage = old;
            return false;

        }


        private readonly ObservableCollection<WorkflowAction> _actions = new();
        private readonly ObservableCollection<string> _highlights = new();
        public ReadOnlyObservableCollection<WorkflowAction> Actions { get; }
        public ReadOnlyObservableCollection<string> Highlights { get; }


        private readonly ReaderWriterLockSlim _lock = new();
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

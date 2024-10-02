using HLab.Base.Fluent;
using HLab.Base.ReactiveUI;
using HLab.Erp.Acl;
using HLab.Erp.Workflows.Extensions;
using HLab.Erp.Workflows.Interfaces;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace HLab.Erp.Workflows.Models
{
    public interface IWorkflowStage : IWorkflowConditionalObject
    {
        string Name { get; }
    }

    public abstract class Workflow<T> : ReactiveModel, IWorkflow<T>
        where T : /*Workflow<T>, */class, IWorkflow<T>
    {
        protected Workflow(object target, IDataLocker locker)
        {
            Actions = new(_actions);
            Highlights = new(_highlights);

            Target = target;
            Locker = locker;

            _caption = this
                .WhenAnyValue(e => e.CurrentStage, selector: s => s.GetCaption(this))
                .ToProperty(this, e => e.Caption);

            _iconPath = this
                .WhenAnyValue(e => e.CurrentStage, selector: s => s.GetIconPath(this))
                .ToProperty(this, e => e.IconPath);

            _subIconPath = this
                .WhenAnyValue(e => e.CurrentStage, selector: s => s.GetSubIconPath(this))
                .ToProperty(this, e => e.SubIconPath);


            if (target is INotifyPropertyChanged n)
                n.PropertyChanged += Target_PropertyChanged;
        }

        void Target_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Update();
        }


        public class Stage : WorkflowConditionalObject<T>, IWorkflowStage
        {
            public static Stage CreateDefault(
                Action<IFluentConfigurator<Stage>> configure,
                [CallerMemberName] string name = "")
            {
                var stage = new Stage(name);
                var configurator = new FluentConfigurator<Stage>(stage);
                configure?.Invoke(configurator);
                AddDefaultStage(stage);
                return stage;
            }

            public static Stage Create(
                Action<IFluentConfigurator<Stage>> configure,
                [CallerMemberName] string name = "")
            {
                var stage = new Stage(name);
                var configurator = new FluentConfigurator<Stage>(stage);

                configure?.Invoke(configurator);

                //if(!stage.HasAction)
                {
                    var c = new Action<IFluentConfigurator<Stage>>(c => c
                        //.WhenStageAllowed(() => stage)
                        .Action(async w => await w.SetStageAsync(() => stage, "", "", false, false)));
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
                    errorAction($"{{Stage needed}} : {stage.GetCaption(null)}");
                }

                return false;
            }

            public override string ToString() => Name;
        }


        public class Action : WorkflowConditionalObject<T>, IWorkflowAction
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

            public WorkflowAction GetAction(T workflow) => new WorkflowAction(workflow, this);
            public override string ToString() => GetAction(null).Caption;
        }

        //Todo : user never set
        public User User { get; set; }
        public object Target { get; }
        public IDataLocker Locker { get; }

        static List<Stage> _workflowStage;
        static List<Action> _workflowAction;
        static List<Stage> WorkflowStages => _workflowStage ??= new List<Stage>();
        static List<Action> WorkflowActions => _workflowAction ??= new List<Action>();

        public static Stage StageFromName(string name) => WorkflowStages.Find(e => e.Name == name) ?? DefaultStage;
        protected void SetStage(Stage stage) => CurrentStage = stage;


        public string Caption => _caption.Value;
        readonly ObservableAsPropertyHelper<string> _caption;

        public string IconPath => _iconPath.Value;
        readonly ObservableAsPropertyHelper<string> _iconPath;

        public string SubIconPath => _subIconPath.Value;
        readonly ObservableAsPropertyHelper<string> _subIconPath;

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
            if (!WorkflowActions.Contains(action))
                WorkflowActions.Add(action);
        }

        public static Stage DefaultStage { get; private set; }

        public Stage CurrentStage
        {
            get => _currentStage;
            set
            {
                if (this.SetAndRaise(ref _currentStage, value) && this is T @this)
                    value?.Action(@this);
            }
        }

        Stage _currentStage;

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


        readonly ObservableCollection<WorkflowAction> _actions = new();
        readonly ObservableCollection<string> _highlights = new();
        public ReadOnlyObservableCollection<WorkflowAction> Actions { get; }
        public ReadOnlyObservableCollection<string> Highlights { get; }


        readonly ReaderWriterLockSlim _lock = new();
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
                    foreach (var h in a.Highlights) _highlights.Add(h);
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
        protected Workflow(TTarget target, IDataLocker locker) : base(target, locker)
        {
        }

        public new TTarget Target => (TTarget)base.Target;

    }
}

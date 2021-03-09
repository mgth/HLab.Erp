using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HLab.Erp.Core;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Workflows
{
    public interface IWorkflowFilterViewModel { }

        public class WorkflowFilterViewModel<TClass>: FilterViewModel, IWorkflowFilterViewModel
    where TClass : class, IWorkflow<TClass>
    {

        private static readonly MethodInfo ContainsMethod = typeof(List<string>).GetMethod("Contains", new[] {typeof(string)});

        public class StageEntry : NotifierBase
        {
            public StageEntry() => H<StageEntry>.Initialize(this);

            public bool Selected
            {
                get => _selected.Get(); 
                set => _selected.Set(value);
            }
            private IProperty<bool> _selected = H<StageEntry>.Property<bool>();

            public IWorkflowStage Stage { get; set; }

            public string IconPath => Stage?.GetIconPath(null);
            public string Caption => Stage?.GetCaption(null);
        }

        public ReadOnlyObservableCollection<StageEntry> List { get; }
        public ObservableCollection<StageEntry> _list = new();

        public WorkflowFilterViewModel()
        {
            List = new (_list);
            H<WorkflowFilterViewModel<TClass>>.Initialize(this);


            var properties = typeof(TClass).GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(p => typeof(IWorkflowStage).IsAssignableFrom(p.FieldType));

            foreach (var p in properties)
            {
                var stage = (IWorkflowStage)p.GetValue(null);
                _list.Add(new StageEntry{Stage = stage});
            }

            //IconPath = $"Icons/Workflows/{typeof(TClass).Name}";
        }

        private ITrigger _ = H<WorkflowFilterViewModel<TClass>>.Trigger(c => c
            .On(e => e.List.Item().Selected)
            .Do(e => e.Update?.Invoke())
        );


        public TClass Selected { get; set; }


        public Expression<Func<T,bool>> Match<T>(Expression<Func<T, string>> getter)
        {
            if (!Enabled/* || string.IsNullOrWhiteSpace(Value)*/) 
                return null; 
            var entity = getter.Parameters[0];
            var value = Expression.Constant(List.Where(e => e.Selected).Select(e => e.Stage.Name).ToList(),typeof(List<string>));

            var ex = Expression.Call(value,ContainsMethod,getter.Body);

            return Expression.Lambda<Func<T, bool>>(ex,entity);
        }

        public Action Update
        {
            get => _update.Get();
            set => _update.Set(value);
        }
        private readonly IProperty<Action> _update = H<WorkflowFilterViewModel<TClass>>.Property<Action>();


        public WorkflowFilterViewModel<TClass> Link<T>(ObservableQuery<T> q, Expression<Func<T, string>> getter)
            where T : class, IEntity
        {
            //var entity = getter.Parameters[0];
            q.AddFilter(Title,()=> Match(getter));
            Update = ()=> q.UpdateAsync();
            return this;
        }

    }

}

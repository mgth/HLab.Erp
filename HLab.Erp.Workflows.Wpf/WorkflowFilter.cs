using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HLab.Erp.Core;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Workflows
{
    public static class WorkflowFilterExtensions
    {
        public static IFilterConfigurator<T, WorkflowFilter<TC>> Link<T, TC>(this IFilterConfigurator<T, WorkflowFilter<TC>> fc, Expression<Func<T, string>> getter) 
            where T : class, IEntity, new()
            where TC : class, IWorkflow<TC>
        {
            fc.CurrentFilter.Link<T>(fc.Target().List, getter);
            return fc;
        }
    }

    public class WorkflowFilter<TClass>: FilterViewModel, IWorkflowFilter
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
            private readonly IProperty<bool> _selected = H<StageEntry>.Property<bool>();

            public IWorkflowStage Stage { get; set; }

            public string IconPath => Stage?.GetIconPath(null);
            public string Caption => Stage?.GetCaption(null);
        }

        public ReadOnlyObservableCollection<StageEntry> List { get; }
        public ObservableCollection<StageEntry> _list = new();

        public WorkflowFilter()
        {
            List = new (_list);
            H<WorkflowFilter<TClass>>.Initialize(this);


            var properties = typeof(TClass).GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(p => typeof(IWorkflowStage).IsAssignableFrom(p.FieldType));

            foreach (var p in properties)
            {
                var stage = (IWorkflowStage)p.GetValue(null);
                _list.Add(new StageEntry{Stage = stage});
            }

            //IconPath = $"Icons/Workflows/{typeof(TClass).Name}";
        }

        private ITrigger _ = H<WorkflowFilter<TClass>>.Trigger(c => c
            .On(e => e.List.Item().Selected)
            .On(e => e.Enabled)
            .Do(e => e.Update?.Invoke())
        );


        public TClass Selected { get; set; }


        public Expression<Func<T,bool>> Match<T>(Expression<Func<T, string>> getter)
        {
            if (!Enabled) return null; 

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
        private readonly IProperty<Action> _update = H<WorkflowFilter<TClass>>.Property<Action>();


        public WorkflowFilter<TClass> Link<T>(ObservableQuery<T> q, Expression<Func<T, string>> getter)
            where T : class, IEntity
        {
            //var entity = getter.Parameters[0];
            q.AddFilter(Header,()=> Match(getter));
            Update = q.Update;
            return this;
        }

    }
}
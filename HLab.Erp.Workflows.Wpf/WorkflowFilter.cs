using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HLab.Erp.Core.ListFilters;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Workflows
{
    public class WorkflowFilter<TClass>: Filter<string>, IWorkflowFilter
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
        private readonly ObservableCollection<StageEntry> _list = new();

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
        }

        private ITrigger _ = H<WorkflowFilter<TClass>>.Trigger(c => c
            .On(e => e.List.Item().Selected)
            .On(e => e.Enabled)
            .Do(e => e.Update?.Invoke())
        );


        public TClass Selected { get; set; }


        public override Expression<Func<T,bool>> Match<T>(Expression<Func<T, string>> getter)
        {
            if (!Enabled) return null; 

            var entity = getter.Parameters[0];
            var value = Expression.Constant(List.Where(e => e.Selected).Select(e => e.Stage.Name).ToList(),typeof(List<string>));

            var ex = Expression.Call(value,ContainsMethod,getter.Body);

            return Expression.Lambda<Func<T, bool>>(ex, entity);
        }

        public override Func<TSource, bool> PostMatch<TSource>(Func<TSource, string> getter)
        {
            throw new NotImplementedException();
        }

    }
}
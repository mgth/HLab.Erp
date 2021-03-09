using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.ListFilters
{

    public class EntityFilter<TClass> : FilterEntityViewModel<TClass>, IEntityFilterViewModel
        where TClass : class, IEntity, IListableModel, new()
    {
        [Import] public EntityFilter(ListableEntityListViewModel<TClass> list) : base(list)
        {
            Title = $"{{{typeof(TClass).Name}}}";
            IconPath = $"Icons/Entities/{typeof(TClass).Name}";
        }
    }
    public class EntityFilter<TClass,TList> : FilterEntityViewModel<TClass>, IEntityFilterViewModel
        where TClass : class, IEntity, IListableModel, new()
    where TList : class, IEntityListViewModel<TClass>
    {
        [Import] public EntityFilter(TList list) : base(list)
        {
            Title = $"{{{typeof(TClass).Name}}}";
            IconPath = $"Icons/Entities/{typeof(TClass).Name}";
        }
    }


    public class FilterEntityViewModel<TClass>: FilterViewModel, IEntityFilterViewModel
    where TClass : class, IEntity
    {

        private static readonly MethodInfo ContainsMethod = typeof(List<int?>).GetMethod("Contains", new[] {typeof(int?)});

        public IEntityListViewModel<TClass> List { get; }

        public FilterEntityViewModel(IEntityListViewModel<TClass> list)
        {
            List = list;
            H<FilterEntityViewModel<TClass>>.Initialize(this);
            List.List.CollectionChanged += List_CollectionChanged;
        }

        private void List_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Update?.Invoke();
        }

        public Type ListClass => List.GetType();

        public TClass Selected { get; set; }


        public Expression<Func<T,bool>> Match<T>(Expression<Func<T, int?>> getter)
        {
            if (!Enabled/* || string.IsNullOrWhiteSpace(Value)*/) 
                return null; 
            var entity = getter.Parameters[0];
            var value = Expression.Constant(List.List.Select(e => (int?)e.Id).ToList(),typeof(List<int?>));

            var ex = Expression.Call(value,ContainsMethod,getter.Body);

            return Expression.Lambda<Func<T, bool>>(ex,entity);
        }

        public Action Update
        {
            get => _update.Get();
            set => _update.Set(value);
        }
        private readonly IProperty<Action> _update = H<FilterEntityViewModel<TClass>>.Property<Action>();


        public FilterEntityViewModel<TClass> Link<T>(ObservableQuery<T> q, Expression<Func<T, int?>> getter)
            where T : class, IEntity
        {
            //var entity = getter.Parameters[0];
            q.AddFilter(Title,()=> Match(getter));
            Update = ()=> q.UpdateAsync();
            return this;
        }

    }
}

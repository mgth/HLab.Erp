using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Documents;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.ListFilters
{
    public class EntityFilterViewModel<TClass>: FilterViewModel, IEntityFilterViewModel
    where TClass : class, IEntity
    {

        private static readonly MethodInfo ContainsMethod = typeof(List<int>).GetMethod("Contains", new[] {typeof(int)});

        public IEntityListViewModel<TClass> List { get; }

        public EntityFilterViewModel(IEntityListViewModel<TClass> list)
        {
            List = list;
            H<EntityFilterViewModel<TClass>>.Initialize(this);
            //List.List.CollectionChanged += List_CollectionChanged;
        }

        private ITrigger _ = H<EntityFilterViewModel<TClass>>.Trigger(c => c
            .On(e => e.List.SelectedIds)
            .On(e => e.Enabled)
            .On(e => e.List.List.Item())
            .Do(e => e.Update?.Invoke())
        );


        //private void List_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    Update?.Invoke();
        //}

        public Type ListClass => List.GetType();

        public TClass Selected { get; set; }


        public Expression<Func<T,bool>> Match<T>(Expression<Func<T, int>> getter)
        {
            if (!Enabled/* || string.IsNullOrWhiteSpace(Value)*/) 
                return e=>true;

            var listId = (List.SelectedIds !=null && List.SelectedIds.Any())
                ? List.SelectedIds.ToList()
                : List.List.Select(e => (int)e.Id).ToList();

            var entity = getter.Parameters[0];


            var value =
                Expression.Constant(listId
                    , typeof(List<int>));

            var ex = Expression.Call(value, ContainsMethod, Expression.Convert(getter.Body, typeof(int)));

            return Expression.Lambda<Func<T, bool>>(ex, entity);
        }

        public Action Update
        {
            get => _update.Get();
            set => _update.Set(value);
        }
        private readonly IProperty<Action> _update = H<EntityFilterViewModel<TClass>>.Property<Action>();


        public EntityFilterViewModel<TClass> Link<T>(ObservableQuery<T> q, Expression<Func<T, int>> getter)
            where T : class, IEntity
        {
            //var entity = getter.Parameters[0];
            q.AddFilter(Title, () => Match(getter));
            Update = q.Update;
            return this;
        }
        //public EntityFilterViewModel<TClass> Link<T>(ObservableQuery<T> q, Func<T, int?> getter)
        //    where T : class, IEntity
        //{
        //    //var entity = getter.Parameters[0];
        //    q.AddFilter(Title, () => Match<T>(e => getter(e)??-1));
        //    Update = async () => await q.UpdateAsync();
        //    return this;
        //}

    }
}

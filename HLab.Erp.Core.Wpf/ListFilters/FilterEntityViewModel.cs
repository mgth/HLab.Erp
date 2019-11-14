using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using HLab.Erp.Core.ViewModels;
using HLab.Erp.Core.ViewModels.EntityLists;
using HLab.Erp.Data;

namespace HLab.Erp.Core.ListFilters
{
    class FilterEntityViewModel<TClass>: FilterViewModel<FilterEntityViewModel<TClass>>
    where TClass : class, IEntity
    {

        private static readonly MethodInfo ContainsMethod = typeof(List<TClass>).GetMethod("Contains", new[] {typeof(TClass)});

        private readonly IListViewModel<TClass> _list;

        public FilterEntityViewModel(IListViewModel<TClass> list)
        {
            _list = list;
        }


        public TClass Selected { get; set; }
        public Expression<Func<T,bool>> Match<T>(Expression<Func<T, TClass>> getter)
        {
            var entity = getter.Parameters[0];
            var value = Expression.Constant(_list.List,typeof(IEnumerable<TClass>));

            var ex = Expression.Call(value,ContainsMethod,getter.Body);

            return Expression.Lambda<Func<T, bool>>(ex,entity);
        }

    }
}

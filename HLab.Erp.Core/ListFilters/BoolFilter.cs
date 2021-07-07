using System;
using System.Linq.Expressions;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.Wpf.ListFilters
{
    public class BoolFilter : Filter<bool?>
    {
        public BoolFilter() => H<BoolFilter>.Initialize(this);

        public override Expression<Func<T, bool>> Match<T>(Expression<Func<T, bool?>> getter)
        {
            if (!Value.HasValue) return t => true;

            var entity = getter.Parameters[0];
            var value = Expression.Constant(Value, typeof(bool?));

            var ex = Expression.Equal(getter.Body, value); //Expression.Call(ex1, ContainsMethod, value);

            return Expression.Lambda<Func<T, bool>>(ex, entity);
        }

        public override Func<TSource, bool> PostMatch<TSource>(Func<TSource, bool?> getter)
            => s => Value == null || getter(s) == Value.Value;

    }
}
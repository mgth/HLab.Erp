using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.ListFilters
{
    using H = H<TextFilter>;

    public class TextFilter : Filter<string>
    {
        public TextFilter() => H.Initialize(this);

        private static readonly MethodInfo ContainsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        private static readonly MethodInfo ToLowerMethod = typeof(string).GetMethod("ToLower", new Type[] { });

        public override Expression<Func<T, bool>> Match<T>(Expression<Func<T, string>> getter)
        {
            if (string.IsNullOrEmpty(Value)) return t => true;

            var entity = getter.Parameters[0];
            var value = Expression.Constant(Value.ToLower(), typeof(string));

            var ex1 = Expression.Call(getter.Body, ToLowerMethod);
            var ex = Expression.Call(ex1, ContainsMethod, value);

            return Expression.Lambda<Func<T, bool>>(ex, entity);
        }

        public override Func<TSource, bool> PostMatch<TSource>(Func<TSource, string> getter)
            => s => Value == null || getter(s).Contains(Value);

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HLab.Base;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.ListFilters
{
    public class EntityFilterNullable<TClass> : Filter<int?>, IEntityFilterNullable<TClass>
    where TClass : class, IEntity, new()
    {
        private static readonly MethodInfo ContainsMethod = typeof(List<int?>).GetMethod("Contains", new[] { typeof(int?) });

        public IEntityListViewModel<TClass> Target { get; }

        public EntityFilterNullable(IEntityListViewModel<TClass> target)
        {
            Target = target;
            H<EntityFilterNullable<TClass>>.Initialize(this);
        }

        private ITrigger _ = H<EntityFilterNullable<TClass>>.Trigger(c => c
            .On(e => e.Target.SelectedIds)
            .On(e => e.Target.List.Item())
            .Do(e => e.Update?.Invoke())
        );

        public Type ListClass => Target.GetType();

        public TClass Selected { get; set; }

        public override Expression<Func<T, bool>> Match<T>(Expression<Func<T, int?>> getter)
        {
            var listId = (Target.SelectedIds != null && Target.SelectedIds.Any())
                ? Target.SelectedIds.Cast<int?>().ToList()
                : Target.List.Select(e => (int?)e.Id).ToList();

            var entity = getter.Parameters[0];

            var value =
                Expression.Constant(listId
                    , typeof(List<int?>));

            var ex = Expression.Call(value, ContainsMethod, Expression.Convert(getter.Body, typeof(int?)));

            //Expression<Func<T, bool>> test = e => e == null;
            //var visitor = new SubstExpressionVisitor { Subst = { [test.Parameters[0]] = entity } };

            //var ex1 = Expression.Condition(visitor.Visit(test.Body), Expression.Constant(false, typeof(bool)), ex);

            return Expression.Lambda<Func<T, bool>>(ex, entity);
        }

        public override Func<T, bool> PostMatch<T>(Func<T, int?> getter)
        {
            var listId = (Target.SelectedIds != null && Target.SelectedIds.Any())
                ? Target.SelectedIds.Cast<int?>().ToList()
                : Target.List.Select(e => (int?)e.Id).ToList();

            return e => listId.Contains(getter(e));
        }

    }
}

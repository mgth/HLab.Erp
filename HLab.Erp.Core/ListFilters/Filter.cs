using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using HLab.Base;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.ListFilters
{
    public abstract class Filter<T> : FilterViewModel
    {
        protected Filter() => H<Filter<T>>.Initialize(this);
        public T Value {
            get => _value.Get();
            set => _value.Set(value);
        }
        private readonly IProperty<T> _value = H<Filter<T>>.Property<T>();

        public Action Update
        {
            get => _update.Get();
            set => _update.Set(value);
        }
        private readonly IProperty<Action> _update = H<Filter<T>>.Property<Action>();

        private IProperty<bool> _updateTrigger = H<Filter<T>>.Property<bool>(c => c
            .On(e => e.Value)
            .On(e => e.Enabled)
            .On(e => e.Update)
            .NotNull(e => e.Update)
            .Do((e, p) => e.Update.Invoke())
        );

        public abstract Expression<Func<TSource, bool>> Match<TSource>(Expression<Func<TSource, T>> getter);
        public abstract Func<TSource, bool> PostMatch<TSource>(Func<TSource, T> getter);

        public void Link<TSource>(ObservableQuery<TSource> q, Expression<Func<TSource, T>> getter)
            where TSource : class, IEntity, new()
        {
            Func<Expression<Func<TSource, bool>>> match = ()=>Match(getter);
            void Enable()
            {
                q.AddFilter(match, 0, Header);
            }
            void Disable()
            {
                q.RemoveFilter(Header);
            }
            enabledAction = Enable;
            disabledAction = Disable;
            Update = q.Update;
        }

        public void PostLink<TSource>(ObservableQuery<TSource> q, Func<TSource, T> getter)
            where TSource : class, IEntity, new()
        {
            enabledAction = () => q.AddPostFilter(Header,PostMatch(getter));
            disabledAction = () => q.RemoveFilter(Header);
            Update = q.Update;
        }

    }
}

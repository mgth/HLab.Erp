using System;
using System.Linq.Expressions;
using HLab.Erp.Data.Observables;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.ListFilters
{
    public abstract class Filter : ViewModel, IFilter
    {
        protected Filter() => H<Filter>.Initialize(this);
        public object Header
        {
            get => _header.Get();
            set => _header.Set(value);
        }
        private readonly IProperty<object> _header = H<Filter>.Property<object>();

        public string IconPath
        {
            get => _iconPath.Get();
            set => _iconPath.Set(value);
        }

        public abstract void Link<T, TSource>(IObservableQuery<TSource> q, Expression<Func<TSource, T>> getter);
        private readonly IProperty<string> _iconPath = H<Filter>.Property<string>();

        public bool Enabled
        {
            get => _enabled.Get();
            set
            {
                if(_enabled.Set(value))
                {
                    if(value) Enable();
                    else Disable();
                }
            }
        }
        private readonly IProperty<bool> _enabled = H<Filter>.Property<bool>();

        protected Action enabledAction;
        protected Action disabledAction;
        protected virtual void Enable() => enabledAction?.Invoke();

        protected virtual void Disable() => disabledAction?.Invoke();

    }


    public abstract class Filter<T> : Filter, IFilter<T>
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

        public override void Link<T1, TSource>(IObservableQuery<TSource> q, Expression<Func<TSource, T1>> getter)
            => Link<TSource>(q, getter as Expression<Func<TSource, T>>);

        public void Link<TSource>(IObservableQuery<TSource> q, Expression<Func<TSource, T>> getter)
//            where TSource : class, IEntity, new()
        {
            Func<Expression<Func<TSource, bool>>> match = ()=>Match(getter);
            void _enable()
            {
                q.AddFilter(match, 0, Header);
            }
            void _disable()
            {
                q.RemoveFilter(Header);
            }
            enabledAction = _enable;
            disabledAction = _disable;
            Update = q.Update;
        }

        public void PostLink<TSource>(IObservableQuery<TSource> q, Func<TSource, T> getter)
//            where TSource : class, IEntity, new()
        {
            enabledAction = () => q.AddPostFilter(Header,PostMatch(getter));
            disabledAction = () => q.RemoveFilter(Header);
            Update = q.Update;
        }

    }
}

using System;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

using HLab.Erp.Data.Observables;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.Wpf.ListFilters
{
    public abstract class Filter : ViewModel, IFilter
    {
        protected Filter() => H<Filter>.Initialize(this);
        public object Header
        {
            get => _header.Get();
            set => _header.Set(value);
        }

        readonly IProperty<object> _header = H<Filter>.Property<object>();

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        readonly IProperty<string> _name = H<Filter>.Property<string>();

        public string IconPath
        {
            get => _iconPath.Get();
            set => _iconPath.Set(value);
        }

        readonly IProperty<string> _iconPath = H<Filter>.Property<string>();

        public abstract void Link<T, TSource>(IObservableQuery<TSource> q, Expression<Func<TSource, T>> getter);

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

        public abstract string StringValue { get; set; }

        readonly IProperty<bool> _enabled = H<Filter>.Property<bool>();

        protected Action enabledAction;
        protected Action disabledAction;
        protected virtual void Enable() => enabledAction?.Invoke();

        protected virtual void Disable() => disabledAction?.Invoke();

        public abstract void FromXml(XElement child);

        public virtual XElement ToXml()
        {
            var filter = new XElement(Name);
//            filter.SetAttributeValue("Name",Name);

            return filter;
        }
    }


    public abstract class Filter<T> : Filter, IFilter<T>
    {
        protected Filter() => H<Filter<T>>.Initialize(this);
        public T Value {
            get => _value.Get();
            set => _value.Set(value);
        }

        readonly IProperty<T> _value = H<Filter<T>>.Property<T>();

        public override string StringValue
        {
            get => Value==null ? "" : Value.ToString();
            set
            {
                if(typeof(T) == typeof(int?))
                {
                    if(int.TryParse(value, out var i))
                    {
                        Value = (T)(object)i;
                    }
                    else Value = default;
                }

                if(typeof(T) == typeof(string))
                {
                    Value = (T)(object)value;
                }
            }
        }

        public Action Update
        {
            get => _update.Get();
            set => _update.Set(value);
        }

        readonly IProperty<Action> _update = H<Filter<T>>.Property<Action>();

        IProperty<bool> _updateTrigger = H<Filter<T>>.Property<bool>(c => c
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

        bool _linked = false;

        public void Link<TSource>(IObservableQuery<TSource> q, Expression<Func<TSource, T>> getter)
//            where TSource : class, IEntity, new()
        {
            if(_linked) {}
            _linked = true;
            Func<Expression<Func<TSource, bool>>> match = ()=>Match(getter);
            void EnableFunc()
            {
                q.AddFilter(match, 0, Header);
            }
            void DisableFunc()
            {
                q.RemoveFilter(Header);
            }
            enabledAction = EnableFunc;
            disabledAction = DisableFunc;
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

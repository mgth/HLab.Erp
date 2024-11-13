using System;
using System.Linq.Expressions;
using System.Xml.Linq;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Mvvm.ReactiveUI;
using ReactiveUI;

namespace HLab.Erp.Core.ListFilters;

public abstract class ListElement : ViewModel
{
    protected ListElement() { }
    public object Header
    {
        get => _header;
        set => this.RaiseAndSetIfChanged(ref _header,value);
    }
    object _header;

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name,value);
    }
    string _name;

    public string IconPath
    {
        get => _iconPath;
        set => this.RaiseAndSetIfChanged(ref _iconPath,value);
    }
    string _iconPath;

}

public abstract class Filter : ListElement, IFilter
{
    protected Filter()
    {
        this.WhenAnyValue(e => e.Enabled).Subscribe(e =>
        {
            if (e) Enable();
            else Disable();
        });

    }

    public abstract void Link<T, TSource>(IObservableQuery<TSource> q, Expression<Func<TSource, T>> getter) where TSource : class, IEntity;

    public bool Enabled
    {
        get => _enabled;
        set => this.RaiseAndSetIfChanged(ref _enabled,value);
    }
    bool _enabled;

    public abstract string StringValue { get; set; }

    protected Action EnabledAction;
    protected Action DisabledAction;
    protected virtual void Enable() => EnabledAction?.Invoke();

    protected virtual void Disable() => DisabledAction?.Invoke();

    public abstract void FromXml(XElement child);

    public virtual XElement ToXml()
    {
        return new XElement(Name);
    }
}


public abstract class Filter<T> : Filter, IFilter<T>
{
    protected Filter()
    {
        this.WhenAnyValue(
            e => e.Value, 
            e => e.Enabled,
            e => e.Update
        ).Subscribe(e => e.Item3?.Invoke());
    }

    public T Value {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value,value);
    }
    T _value;

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
        get => _update;
        set => this.RaiseAndSetIfChanged(ref _update,value);
    }
    Action _update;

    public abstract Expression<Func<TSource, bool>> Match<TSource>(Expression<Func<TSource, T>> getter);
    public abstract Func<TSource, bool> PostMatch<TSource>(Func<TSource, T> getter);

    public override void Link<T1, TSource>(IObservableQuery<TSource> q, Expression<Func<TSource, T1>> getter)
        => Link<TSource>(q, getter as Expression<Func<TSource, T>>);

    bool _linked = false;

    public void Link<TSource>(IObservableQuery<TSource> q, Expression<Func<TSource, T>> getter)
            where TSource : class, IEntity
    {
        if(_linked) {}
        _linked = true;

        EnabledAction = () => q.AddFilter(() => Match(getter), 0, Header);
        DisabledAction = () => q.RemoveFilter(Header);

        Update = q.Update;
    }

    public void PostLink<TSource>(IObservableQuery<TSource> q, Func<TSource, T> getter)
            where TSource : class, IEntity
    {
        EnabledAction = () => q.AddPostFilter(Header,PostMatch(getter));
        DisabledAction = () => q.RemoveFilter(Header);
        Update = q.Update;
    }

}
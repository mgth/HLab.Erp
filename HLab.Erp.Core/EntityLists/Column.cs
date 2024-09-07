#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;

namespace HLab.Erp.Core.EntityLists;

public class Column<T> : ListElement, IColumn<T> where T : class, IEntity
{
    internal Column()
    {
        Name = "C" + Guid.NewGuid().ToString().Replace('-', '_');
    }

    public double Width { get; set; } = double.NaN;
    public bool Hidden { get; set; } = false;

    public Func<T, object>? OrderBy { get;  set; }
    public int OrderByRank { get; set; } = -1;

    public SortDirection SortDirection { get;  set; }

    public IColumn<T>? OrderByNext { get; set; }

    // TODO
    //readonly List<TriggerPath> _triggerPaths = new();

    public void AddTrigger(Expression expression)
    {
        try
        {
            //TODO
            //_triggerPaths.Add(TriggerPath.Factory(expression));
        }
        catch (Exception e)
        {

        }
    }

    //readonly ConditionalWeakTable<T,object> _cache = new();

    public object? DataTemplate { get ; set ; }

    public override string ToString()
    {
        return Header?.ToString()??"";
    }

    public void RegisterTriggers(T model, Action<string> handler)
    {
        //TODO
        //foreach(var path in _triggerPaths)
        //{
        //    path.GetTrigger(NotifyClassHelper.GetHelper(model),(s,e)=>handler(Name));
        //}
    }


}
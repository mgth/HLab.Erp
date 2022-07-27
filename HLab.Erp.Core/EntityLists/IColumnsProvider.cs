using System;
using System.Collections.Generic;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Data.Observables;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.EntityLists
{
    public interface IListElementViewClass : IViewClass {}

    public interface IColumnsProvider
    {
        void Populate(object grid);

        Dictionary<string, IColumn> Columns {get;} 
    }

    public interface IColumnsProvider<T> : IColumnsProvider
    {
        object GetValue(T obj, string name);

        //object GetView();

        void AddColumn(IColumn<T> column);

        IObservableQuery<T> List {get;}

        void RegisterTriggers(T model, Action<string> handler);
    }

}

using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Mvvm.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HLab.Erp.Core.Wpf.EntityLists
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

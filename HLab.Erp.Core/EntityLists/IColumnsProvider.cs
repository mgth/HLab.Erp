using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.EntityLists
{
    public interface IListElementViewClass : IViewClass {}

    public interface IColumnsProvider
    {
        void Populate(object grid);
        object BuildTemplate(string template);
        Dictionary<string, IColumn> Columns {get;} 
    }

    public interface IColumnsProvider<T> : IColumnsProvider where T : class, IEntity
    {
        void SetDefaultOrderBy();
        bool GetValue(T obj, string name, out object result);

        //object GetView();

        void AddColumn(IColumn<T> column);
        string AddProperty<TOut>(Expression<Func<T, TOut>> getter);
        string AddProperty(Func<T,bool> canEvaluate, Func<T, object> getter);
        string AddProperty(string name, Func<T,bool> canEvaluate, Func<T, object> getter);


        IObservableQuery<T> List {get;}

        void RegisterTriggers(T model, Action<string> handler);
    }

}

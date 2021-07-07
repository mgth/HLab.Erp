using HLab.Erp.Data.Observables;
using System;
using System.Linq.Expressions;

namespace HLab.Erp.Core.Wpf.ListFilters
{
    public interface IEntityFilterViewModel : IFilter { }

    public interface IEntityFilterNullable<TE> : IEntityFilterViewModel
//        where TE : class, IEntity, new()
    { 
        IEntityListViewModel<TE> Target {get; }

        void Link<T>(IObservableQuery<T> list, Expression<Func<T, int?>> getter);
        void PostLink<T>(IObservableQuery<T> list, Func<T, int?> getter);
    }
    public interface IEntityFilterNotNull<TE> : IEntityFilterViewModel
//        where TE : class, IEntity, new()
    { 
        IEntityListViewModel<TE> Target {get; }

        void Link<T>(IObservableQuery<T> list, Expression<Func<T, int>> getter);
        void PostLink<T>(IObservableQuery<T> list, Func<T, int> getter);
    }
}
using System;
using System.Linq.Expressions;
using HLab.Erp.Data.Observables;

namespace HLab.Erp.Core.ListFilters
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
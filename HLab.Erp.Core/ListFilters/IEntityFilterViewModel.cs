using System;
using System.Linq.Expressions;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;

namespace HLab.Erp.Core.ListFilters
{
    public interface IEntityFilterViewModel : IFilter { }

    public interface IEntityFilterNullable<TE> : IEntityFilterViewModel where TE : class, IEntity
    //        where TE : class, IEntity, new()
    { 
        IEntityListViewModel<TE> Target {get; }

        void Link<T>(IObservableQuery<T> list, Expression<Func<T, int?>> getter) where T : class, IEntity;
        void PostLink<T>(IObservableQuery<T> list, Func<T, int?> getter) where T : class, IEntity;
    }
    public interface IEntityFilterNotNull<TE> : IEntityFilterViewModel where TE : class, IEntity
    //        where TE : class, IEntity, new()
    { 
        IEntityListViewModel<TE> Target {get; }

        void Link<T>(IObservableQuery<T> list, Expression<Func<T, int>> getter) where T : class, IEntity;
        void PostLink<T>(IObservableQuery<T> list, Func<T, int> getter) where T : class, IEntity;
    }
}
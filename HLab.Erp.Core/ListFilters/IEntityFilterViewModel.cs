using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using System;
using System.Linq.Expressions;

namespace HLab.Erp.Core.ListFilters
{
    public interface IEntityFilterViewModel : IFilterViewModel { }

    public interface IEntityFilterNullable<TE> : IEntityFilterViewModel
        where TE : class, IEntity, new()
    { 
        IEntityListViewModel<TE> Target {get; }

        void Link<T>(ObservableQuery<T> list, Expression<Func<T, int?>> getter) where T : class, IEntity, new();
        void PostLink<T>(ObservableQuery<T> list, Func<T, int?> getter) where T : class, IEntity, new();
    }
    public interface IEntityFilterNotNull<TE> : IEntityFilterViewModel
        where TE : class, IEntity, new()
    { 
        IEntityListViewModel<TE> Target {get; }

        void Link<T>(ObservableQuery<T> list, Expression<Func<T, int>> getter) where T : class, IEntity, new();
        void PostLink<T>(ObservableQuery<T> list, Func<T, int> getter) where T : class, IEntity, new();
    }
}
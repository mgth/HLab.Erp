using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using System;
using System.Linq.Expressions;

namespace HLab.Erp.Core
{
    public interface IFilter
    {
        bool Enabled { get; set; }
        object Header { get; set; }
        string IconPath { get; set; }
        void Link<T, TSource>(IObservableQuery<TSource> q, Expression<Func<TSource, T>> getter);

    }

    public interface IFilter<T> : IFilter
    {
        void Link<TSource>(IObservableQuery<TSource> q, Expression<Func<TSource, T>> getter);
    }
}

using HLab.Erp.Data.Observables;
using System;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace HLab.Erp.Core
{
    public interface IFilter
    {

        bool Enabled { get; set; }
        object Header { get; set; }
        string IconPath { get; set; }
        string StringValue { get; set; }
        string Name { get; set; }

        void Link<T, TSource>(IObservableQuery<TSource> q, Expression<Func<TSource, T>> getter);
        void FromXml(XElement child);
        XElement ToXml();
    }

    public interface IFilter<T> : IFilter
    {
        void Link<TSource>(IObservableQuery<TSource> q, Expression<Func<TSource, T>> getter);
        void PostLink<TSource>(IObservableQuery<TSource> q, Func<TSource, T> getter);
    }
}

using System;
using System.Linq.Expressions;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.EntityLists
{
    public interface IColumnBuilder<T, TLink, out TFilter>
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        IEntityListViewModel<T> ListViewModel { get; }
        IColumn<T> Column { get; }
        TFilter Filter { get; }
        int OrderByRank { get; set; }
    }


    public interface IColumnConfigurator : IDisposable
    {
        ILocalizationService Localization { get; }
        string Localize(string s) => Localization.Localize(s);
    }

    public interface IColumnConfigurator<T> : IColumnConfigurator
        where T : class, IEntity, new()
    {
        IColumnConfigurator<T, object, IFilter<object>> GetColumnConfigurator();
        IColumnConfigurator<T> BuildList(Action<IEntityListViewModel<T>> build);
        IColumnConfigurator<T> List<TT>(out TT list) where TT : IEntityListViewModel<T>;
    }

    public interface IColumnConfigurator<T, TLink> : IColumnConfigurator<T>
        where T : class, IEntity, new()
    {
        IColumnConfigurator<T, TLinkChild, TFilterChild> GetFilterConfigurator<TLinkChild, TFilterChild>()
            where TFilterChild : class, IFilter<TLinkChild>;
        Expression<Func<T, TLink>> LinkExpression { get; set; }
        Func<T, TLink> LinkLambda { get; set; }
        int OrderByRank { get; set; }
    }

    public interface IColumnConfigurator<T, TLink, out TFilter> :
        IColumnConfigurator<T, TLink>
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        IColumnConfigurator<T, TLink, TFilter> Build(Action<IColumnBuilder<T, TLink, TFilter>> builder);
        IColumnConfigurator<T, TLink, TFilter> FilterLink(Expression<Func<T,TLink>> getter);
        IColumnConfigurator<T, TLink, TFilter> FilterPostLink(Func<T,TLink> getter);
        IColumnConfigurator<T, TLink, TFilter> AddProperty<TOut>(Expression<Func<T,TOut>> getter, out string name);
        IColumnConfigurator<T, TLink, TFilter> AddProperty(Func<T, bool> condition, Func<T,object> getter, out string name);
        IColumnConfigurator<T, TLink, TFilter> AddProperty(string name, Func<T, bool> condition, Func<T,object> getter);
        IColumnConfigurator<T, TLink, TFilter> DecorateTemplate(string template);
        IColumnConfigurator<T, TLink, TFilter> ContentTemplate(string template);

    }

    public class ColumnConfigurationException : Exception
    {
        public ColumnConfigurationException(string message) : base(message)
        {
        }
    }
}
using System;
using System.Linq.Expressions;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;

namespace HLab.Erp.Core.EntityLists
{
    public interface IColumnConfigurator : IDisposable
    {
        IEntityListViewModel Target { get; }
        IColumn Column { get; }
        IErpServices Erp { get; }

        string Localize(string s) => Erp.Localization.Localize(s);
    }

    public interface IColumnConfigurator<T> : IColumnConfigurator
        where T : class, IEntity, new()
    {
        new IEntityListViewModel<T> Target { get; }
        new IColumn<T> Column { get; }
        IColumnConfigurator<T, object, IFilter<object>> GetNewConfigurator();
        IColumnConfigurator<T, TLinkChild, TFilterChild> GetChildConfigurator<TLinkChild, TFilterChild>()
            where TFilterChild : IFilter<TLinkChild>;
    }

    public interface IColumnConfigurator<T, TLink> : IColumnConfigurator<T>
        where T : class, IEntity, new()
    {
        Expression<Func<T, TLink>> LinkExpression { get; set; }
        Func<T, TLink> LinkLambda { get; set; }
    }

    public interface IColumnConfigurator<T, TLink, out TFilter> :
        IColumnConfigurator<T, TLink>
        where T : class, IEntity, new()
        where TFilter : IFilter<TLink>
    {
        TFilter Filter { get; }


    }

    public class ColumnConfigurationException : Exception
    {
        public ColumnConfigurationException(string message) : base(message)
        {
        }
    }
}
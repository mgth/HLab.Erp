using System;
using System.Linq.Expressions;
using HLab.Erp.Data;

namespace HLab.Erp.Core.EntityLists
{

    public interface IColumn
    {
        string Id { get; set; }
        bool Hidden { get; set; }
        object Header { get; set; }
        string IconPath { get; set; }
        double Width { get; set; }
        SortDirection SortDirection { get; set; }

        public interface IHelper
        {
            IColumn Column { get; }
            IEntityListViewModel Target { get; }
        }
    }

    public interface IColumn<T> : IColumn
    {
        Func<T, object> OrderBy { get; set; }
        Func<T, object> Getter { get; set; }

        IColumn<T> OrderByNext { get; set; }

        object GetValue(T target);

        public new interface IHelper : IColumn.IHelper
        {
            new IEntityListViewModel<T> Target { get; }
            new IColumn<T> Column { get; }

            TFilter GetFilter<TFilter>() where TFilter : IFilter;

            Expression Link { get; set; }
            Delegate PostLink { get; set; }

            Expression<Func<T, TLink>> GetLinkExpression<TLink>() => (Expression<Func<T, TLink>>)Link;

            IErpServices Erp { get; }
        }
    }
}
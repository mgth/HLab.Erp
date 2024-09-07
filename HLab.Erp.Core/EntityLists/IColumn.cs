using System;
using System.Linq.Expressions;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;

namespace HLab.Erp.Core.EntityLists
{
    public interface IColumn
    {
        string Name { get; set; }
        object Header { get; set; }
        string IconPath { get; set; }
        double Width { get; set; }
        bool Hidden { get; set; }

        SortDirection SortDirection { get; set; }
        int OrderByRank { get; set; }

        object DataTemplate { get; set; }

        public interface IBuilder
        {
            string DataTemplateSource { get; set; }
            IColumn Column { get; }
            IEntityListViewModel ListViewModel { get; }
            void Build();
        }
    }

    public interface IColumn<T> : IColumn where T : class, IEntity
    {
        Func<T, object> OrderBy { get; set; }

        void AddTrigger(Expression expression);

        IColumn<T> OrderByNext { get; set; }

        public new interface IBuilder : IColumn.IBuilder
        {
            new IEntityListViewModel<T> ListViewModel { get; }
            new IColumn<T> Column { get; }

            TFilter GetFilter<TFilter>() where TFilter : IFilter;

            Expression Link { get; set; }
            Delegate PostLink { get; set; }

            Expression<Func<T, TLink>> GetLinkExpression<TLink>() => (Expression<Func<T, TLink>>)Link;

            public int OrderByRank { get; set; }

        }

        void RegisterTriggers(T model, Action<string> handler);
    }
}
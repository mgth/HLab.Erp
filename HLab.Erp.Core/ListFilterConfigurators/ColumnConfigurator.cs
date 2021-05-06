using System;
using System.Diagnostics;
using System.Linq.Expressions;
using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;

namespace HLab.Erp.Core.ListFilterConfigurators
{

    public class ColumnConfigurator<T, TLink, TFilter> : 
        IColumnConfigurator<T, TLink, TFilter>
        
        
        where T : class, IEntity, new()
        where TFilter : IFilter<TLink>
    {
        public ColumnConfigurator(IEntityListViewModel<T> list)
            : this(new ColumnHelper<T>(new Column<T>(),list))
        {
        }

        public IColumnConfigurator<T, TLinkChild, TFilterChild> GetChildConfigurator<TLinkChild, TFilterChild>()
        where TFilterChild : IFilter<TLinkChild>
        {
            return new ColumnConfigurator<T, TLinkChild, TFilterChild>(Helper);
        }

        protected ColumnConfigurator(IColumn<T>.IHelper helper)
        {
            Helper = helper;

            var filter = helper.GetFilter<TFilter>();
            if (filter == null) return;

            var link = helper.GetLinkExpression<TLink>();
            if (link == null) return;

            filter?.Link(helper.Target.List, link);
        }

        public TFilter Filter => Helper.GetFilter<TFilter>();
        public Expression<Func<T, TLink>> Link { get => Helper.Link as Expression<Func<T, TLink>> ; set => Helper.Link = value; }

        protected IColumn<T>.IHelper Helper { get; }

        public IEntityListViewModel<T> Target => Helper.Target;
        IEntityListViewModel IColumnConfigurator.Target => Helper.Target;

        public IColumn<T> Column => Helper.Column;
        IColumn IColumnConfigurator.Column => Helper.Column;



        public void Dispose()
        {
            if (Column.Getter == null)
            {
                var link = Link;
                if (link != null)
                {
                    var lambda = link.Compile();
                    Column.Getter = t => lambda(t); //TODO manipulate expression before
                    Target.Columns.AddColumn(Column);
                }
            }
            else        
                Target.Columns.AddColumn(Column);

            if (Filter != null)
                Target.AddFilter(Filter);
        }


    }
}
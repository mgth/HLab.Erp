using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Grace.DependencyInjection.Attributes;
using HLab.Base.Extensions;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;

namespace HLab.Erp.Core.ListFilterConfigurators
{
    [Export( typeof(IColumnConfigurator<,,>))]
    public class ColumnConfigurator<T, TLink, TFilter> :  IColumnConfigurator<T, TLink, TFilter>
        
        where T : class, IEntity, new()
        where TFilter : IFilter<TLink>
    {
        private readonly Func<IColumn<T>, IEntityListViewModel<T>, IColumn<T>.IHelper> _getHelper;
        public ColumnConfigurator(IEntityListViewModel<T> list, Func<IColumn<T>, IEntityListViewModel<T>, IColumn<T>.IHelper> getHelper)
            : this(getHelper(new Column<T>(),list),getHelper)
        {
            _getHelper = getHelper;
        }

        public IColumnConfigurator<T, TLinkChild, TFilterChild> GetChildConfigurator<TLinkChild, TFilterChild>()
        where TFilterChild : IFilter<TLinkChild>
        {
            return new ColumnConfigurator<T, TLinkChild, TFilterChild>(Helper,_getHelper);
        }

        public IColumnConfigurator<T, object, IFilter<object>> GetNewConfigurator()
        {
            return new ColumnConfigurator<T, object, IFilter<object>>(_getHelper(new Column<T>(), Target), _getHelper);
        }

        public IErpServices Erp => Helper.Erp;

        protected ColumnConfigurator(IColumn<T>.IHelper helper, Func<IColumn<T>, IEntityListViewModel<T>, IColumn<T>.IHelper> getHelper)
        {
            _getHelper = getHelper;
            Helper = helper;

            var filter = helper.GetFilter<TFilter>();
            if (filter == null) return;

            var link = helper.GetLinkExpression<TLink>();
            if (link == null) return;

            filter?.Link(helper.Target.List, link);
        }



        public TFilter Filter => Helper.GetFilter<TFilter>();
        public Expression<Func<T, TLink>> Link
        {
            get => Helper.Link as Expression<Func<T, TLink>> ;
            set
            {
                var lambda = value.CastReturn(default(object)).Compile(); 
                Helper.Column.OrderBy ??= lambda; 
                Helper.Link = value;
            }
        }

        protected IColumn<T>.IHelper Helper { get; }

        public IEntityListViewModel<T> Target => Helper.Target;
        IEntityListViewModel IColumnConfigurator.Target => Helper.Target;

        public IColumn<T> Column => Helper.Column;
        IColumn IColumnConfigurator.Column => Helper.Column;

        public Task<string> Localize(string s) => Helper.Erp.Localization.LocalizeAsync(s);


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
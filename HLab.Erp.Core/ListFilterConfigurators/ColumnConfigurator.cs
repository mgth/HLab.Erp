using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HLab.Base.Extensions;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.ListFilterConfigurators
{
    public class ColumnConfigurator<T, TLink, TFilter> : IColumnConfigurator<T, TLink, TFilter>

        where T : class, IEntity, new()
        where TFilter : IFilter<TLink>
    {
        public ColumnConfigurator(IEntityListViewModel<T> list, ILocalizationService localization)
            : this(new ColumnHelper<T>(new Column<T>(), list), localization)
        {
        }

        public IColumnConfigurator<T, TLinkChild, TFilterChild> GetChildConfigurator<TLinkChild, TFilterChild>()
        where TFilterChild : IFilter<TLinkChild>
        {
            return new ColumnConfigurator<T, TLinkChild, TFilterChild>(Helper,Localization);
        }

        public IColumnConfigurator<T, object, IFilter<object>> GetNewConfigurator()
        {
            return new ColumnConfigurator<T, object, IFilter<object>>(new ColumnHelper<T>(new Column<T>(), Target),Localization);
        }

        protected ColumnConfigurator(IColumn<T>.IHelper helper, ILocalizationService localization)
        {
            Helper = helper;
            Localization = localization;

            var filter = helper.GetFilter<TFilter>();
            if (filter == null) return;

            var link = helper.GetLinkExpression<TLink>();
            if (link == null) return;

            filter?.Link(helper.Target.List, link);
        }

        public TFilter Filter => Helper.GetFilter<TFilter>();
        public Expression<Func<T, TLink>> LinkExpression
        {
            get => Helper.Link as Expression<Func<T, TLink>>;
            set
            {
                var lambda = value.CastReturn(default(object)).Compile();
                Helper.Column.OrderBy ??= lambda;
                Helper.Link = value;
            }
        }

        public Func<T, TLink> LinkLambda
        {
            get => Helper.PostLink as Func<T, TLink>;
            set
            {
                Helper.Column.OrderBy ??= t => value(t);
                Helper.PostLink = value;
            }
        }

        protected IColumn<T>.IHelper Helper { get; }

        public IEntityListViewModel<T> Target => Helper.Target;
        IEntityListViewModel IColumnConfigurator.Target => Helper.Target;

        public IColumn<T> Column => Helper.Column;
        public ILocalizationService Localization { get; }
        IColumn IColumnConfigurator.Column => Helper.Column;

        public Task<string> Localize(string s) => Localization.LocalizeAsync(s);


        public void Dispose()
        {
            if (Column.Getter == null)
            {
                var link = LinkExpression;
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
using System;
using System.Linq.Expressions;
using HLab.Erp.Core.Wpf.EntityLists;

namespace HLab.Erp.Core.ListFilterConfigurators
{
    public class ColumnHelper<T> : IColumn<T>.IHelper
    {
        public IErpServices Erp { get; }
        public ColumnHelper(IColumn<T> column, IEntityListViewModel<T> target, IErpServices erp)
        {
            Column = column;
            Target = target;
            Erp = erp;
        }

        public IEntityListViewModel<T> Target { get; }
        public IColumn<T> Column { get; }

        private IFilter _filter;

        public TFilter GetFilter<TFilter>() where TFilter : IFilter
        {
            if (_filter != null) return (TFilter) _filter;

            var t = typeof(TFilter);
            if (!t.IsClass || t.IsAbstract) return default;

            var filter = Target.GetFilter<TFilter>();
            
            filter.Header = Column.Header;
            filter.IconPath = Column.IconPath;
            filter.Name = Column.Id;

            _filter = filter;
            return filter;
        }

        public Expression Link { get; set; }
        public Delegate PostLink { get; set; }

        IColumn IColumn.IHelper.Column => Column;
        IEntityListViewModel IColumn.IHelper.Target => Target;
    }
}
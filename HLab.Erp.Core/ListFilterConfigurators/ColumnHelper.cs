using System;
using System.Linq.Expressions;
using HLab.Erp.Core.EntityLists;

namespace HLab.Erp.Core.ListFilterConfigurators
{
    public class ColumnHelper<T> : IColumn<T>.IHelper
    {
        public ColumnHelper(IColumn<T> column, IEntityListViewModel<T> target)
        {
            Column = column;
            Target = target;
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

            _filter = filter;
            return filter;
        }

        public Expression Link { get; set; }


        IColumn IColumn.IHelper.Column => Column;
        IEntityListViewModel IColumn.IHelper.Target => Target;
    }
}
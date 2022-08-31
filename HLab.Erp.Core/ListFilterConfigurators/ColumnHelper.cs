using System;
using System.Linq.Expressions;
using HLab.Base;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;

namespace HLab.Erp.Core.ListFilterConfigurators
{
    public class ColumnBuilder<T> : IColumn<T>.IBuilder
    {
        public ColumnBuilder(IEntityListViewModel<T> listViewModel, IColumn<T> column)
        {
            Column = column;
            ListViewModel = listViewModel;
        }

        public IEntityListViewModel<T> ListViewModel { get; }
        public string DataTemplateSource { get; set; } = XamlTool.ContentPlaceHolder;
        public IColumn<T> Column { get; }
        IFilter _filter;

        public TFilter GetFilter<TFilter>() where TFilter : IFilter
        {
            if (_filter != null) return (TFilter) _filter;

            var t = typeof(TFilter);
            if (!t.IsClass || t.IsAbstract) return default;

            var filter = ListViewModel.GetFilter<TFilter>();
            
            filter.Header = Column.Header;
            filter.IconPath = Column.IconPath;
            filter.Name = Column.Name;

            _filter = filter;
            return filter;
        }

        public Expression Link { get; set; }
        public Delegate PostLink { get; set; }
        public int OrderByRank { get; set; }

        public void Build()
        {
            Column.DataTemplate = ListViewModel.Columns.BuildTemplate(DataTemplateSource);
            ListViewModel.Columns.AddColumn(Column);

            if (_filter != null)
                ListViewModel.AddFilter(_filter);

        }



        IColumn IColumn.IBuilder.Column => Column;
        IEntityListViewModel IColumn.IBuilder.ListViewModel => ListViewModel;
    }
}
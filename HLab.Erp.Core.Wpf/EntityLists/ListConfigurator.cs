using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.EntityLists
{

    public class ListConfigurator<T> : IMainListConfigurator<T>, IDisposable where T : class, IEntity, new()
    {
        private readonly EntityListViewModel<T> _list;
        private IColumnConfigurator<T> _columnConfigurator;
        private IFilterConfigurator<T> _filterConfigurator;

        public ListConfigurator(EntityListViewModel<T> list)
        {
            _list = list;
        }

        public IMainListConfigurator<T> AddAllowed()
        {
            _list.AddAllowed = true;
            return this;
        }

        public IMainListConfigurator<T> DeleteAllowed()
        {
            _list.DeleteAllowed = true;
            return this;
        }

        public IMainListConfigurator<T> ImportAllowed()
        {
            _list.ImportAllowed = true;
            return this;
        }

        public IMainListConfigurator<T> StaticFilter(Expression<Func<T, bool>> filter)
        {
            _list.List.AddFilter(filter);
            return this;
        }

        public IMainListConfigurator<T> ExportAllowed()
        {
            _list.ExportAllowed = true;
            return this;
        }


        public IColumnConfigurator<T> Column()
        {
            _columnConfigurator?.Dispose();
            _columnConfigurator = new ColumnConfigurator(this);
            return _columnConfigurator;
        }

        public IFilterConfigurator<T, TF> Filter<TF>() where TF : class, IFilterViewModel
        {
            _filterConfigurator?.Dispose();
            var filterConfigurator = new FilterConfigurator<TF>(this);
            _filterConfigurator = filterConfigurator;
            return filterConfigurator;
        }

        internal void AddColumn(IColumn<T> column)
        {
            _list.Columns.AddColumn(column);
        }
        internal void AddFilter(IFilterViewModel filter)
        {
            _list.AddFilter(filter);
        }

        public IEntityListViewModel<T> Target() => _list;

        public IMainListConfigurator<T> Header(object header)
        {
            _list.Header = header;
            return this;
        }

        public void Dispose()
        {
            _columnConfigurator?.Dispose();
            _filterConfigurator?.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TF"></typeparam>
        public class FilterConfigurator<TF> : IFilterConfigurator<T, TF>
            where TF : class, IFilterViewModel
        {
            private readonly ListConfigurator<T> _listConfigurator;
            public TF CurrentFilter { get; }

            public FilterConfigurator(ListConfigurator<T> listConfigurator)
            {
                _listConfigurator = listConfigurator;
                CurrentFilter = _listConfigurator.Target().GetFilter<TF>();
            }

            public void Dispose()
            {
                _listConfigurator.AddFilter(CurrentFilter);
            }

            public IFilterConfigurator<T, TF> IconPath(string path)
            {
                CurrentFilter.IconPath = path;
                return this;

            }

            public IFilterConfigurator<T, TF> Header(object header)
            {
                CurrentFilter.Header = header;
                return this;
            }

            public IEntityListViewModel<T> Target() => _listConfigurator.Target();
            public IColumnConfigurator<T> Column() => _listConfigurator.Column();
            IFilterConfigurator<T, TF1> IListConfigurator<T>.Filter<TF1>() => _listConfigurator.Filter<TF1>();

        }
        public class ColumnConfigurator : IColumnConfigurator<T>, IDisposable,
            IStringColumnConfigurator<T>, IDateTimeColumnConfigurator<T>,
            IForeignNotNullColumnConfigurator<T>,
            IForeignNullableColumnConfigurator<T>
        {
            public IColumn<T> CurrentColumn { get; }

            private ListConfigurator<T> _listConfigurator;

            public ColumnConfigurator(ListConfigurator<T> listConfigurator)
            {
                _listConfigurator = listConfigurator;
                CurrentColumn = new Column<T>();
            }

            public IColumnConfigurator<T> Header(object caption)
            {
                CurrentColumn.Header = caption;
                return this;
            }

            public IColumnConfigurator<T> Width(double width)
            {
                CurrentColumn.Width = width;
                return this;
            }

            public IColumnConfigurator<T> Id(string id)
            {
                CurrentColumn.Id = id;
                return this;
            }


            public IColumnConfigurator<T> OrderBy(Func<T, object> orderBy)
            {
                CurrentColumn.OrderBy = orderBy;
                return this;
            }
            public IColumnConfigurator<T> OrderByOrder(int order)
            {
                //foreach (var column in _dict.Values.Where(c => c.OrderByOrder >= order).ToArray())
                //{
                //    column.OrderByOrder++;
                //}
                //CurrentColumn.OrderByOrder = order;
                return this;
            }

            public IColumnConfigurator<T> Hidden()
            {
                    CurrentColumn.Hidden = true;
                    return this;
            }

            Expression<Func<T, string>> IStringColumnConfigurator<T>.Getter => _stringLink;

            Expression<Func<T, DateTime?>> IDateTimeColumnConfigurator<T>.Getter => _dateTimeLink;

            Expression<Func<T, int>> IForeignNotNullColumnConfigurator<T>.Getter => _foreignNotNullLink;

            Expression<Func<T, int?>> IForeignNullableColumnConfigurator<T>.Getter => _foreignLink;



            public IColumnConfigurator<T> Mvvm<TViewClass>() where TViewClass : IViewClass
            {
                CurrentColumn.Getter = o => new ViewLocator { ViewClass = typeof(TViewClass), DataContext = o };
                return this;
            }

            public IColumnConfigurator<T> Column() => _listConfigurator.Column();
            IFilterConfigurator<T, IEntityFilterNotNull<TE>> IForeignNotNullColumnConfigurator<T>.EntityFilter<TE>()
            {
                return Filter<EntityFilter<TE>>().Header(CurrentColumn.Header).Link(((IForeignNotNullColumnConfigurator<T>)this).Getter);
            }
            IFilterConfigurator<T, IEntityFilterNullable<TE>> IForeignNullableColumnConfigurator<T>.EntityFilter<TE>()
            {
                return Filter<EntityFilterNullable<TE>>().Header(CurrentColumn.Header).Link(((IForeignNullableColumnConfigurator<T>)this).Getter);
            }


            public void Dispose()
            {
                Debug.Assert(CurrentColumn.Getter != null);
                _listConfigurator.AddColumn(CurrentColumn);
            }

            public IFilterConfigurator<T, TF> Filter<TF>() where TF : class, IFilterViewModel
                => _listConfigurator.Filter<TF>()
                .Header(CurrentColumn.Header)
                ;

            public IEntityListViewModel<T> Target()
            {
                throw new NotImplementedException();
            }

            private Expression<Func<T, string>> _stringLink;
            private Expression<Func<T, DateTime?>> _dateTimeLink;
            private Expression<Func<T, int?>> _foreignLink;
            private Expression<Func<T, int>> _foreignNotNullLink;

            IStringColumnConfigurator<T> IColumnConfigurator<T>.Link(Expression<Func<T, string>> link)
            {
                if (CurrentColumn.Getter == null)
                {
                    CurrentColumn.Getter = link.Compile();
                }
                _stringLink = link;
                return this;
            }

            IDateTimeColumnConfigurator<T> IColumnConfigurator<T>.Link(Expression<Func<T, DateTime?>> link)
            {
                if (CurrentColumn.Getter == null)
                {
                    CurrentColumn.Getter = e => link.Compile()(e);
                }
                _dateTimeLink = link;
                return this;
            }

            IForeignNotNullColumnConfigurator<T> IColumnConfigurator<T>.Link(Expression<Func<T, int>> link)
            {
                _foreignNotNullLink = link;
                return this;
            }

            IForeignNullableColumnConfigurator<T> IColumnConfigurator<T>.Link(Expression<Func<T, int?>> link)
            {
                _foreignLink = link;
                return this;
            }
        }
    }

}
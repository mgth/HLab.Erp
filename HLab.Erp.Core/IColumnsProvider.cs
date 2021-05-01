using System;
using System.Linq.Expressions;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Core
{
    public interface IListElementViewClass : IViewClass {}

    public interface IColumn<T>
    {
        string Id { get; set; }
        bool Hidden { get; set; }
        object Header { get; set; }
        double Width { get; set; }
        Func<T, object> OrderBy { get; set; }
        int OrderByOrder { get; set; }
        bool OrderDescending { get; set; }
        void AddGetter(Func<T, object, object> getter);
        Func<T, object> Getter { get; set; }
        object GetValue(T target);

        public interface IConfigurator 
        {
            IConfigurator Header(object caption);
            IConfigurator Width(double width);
            IConfigurator Id(string caption);
            IConfigurator OrderBy(Func<T, object> orderBy);
            IConfigurator OrderByOrder(int order);
            IConfigurator Hidden();

            IConfigurator Link(Expression<Func<T, DateTime?>> link);
            IConfigurator Link(Expression<Func<T, int?>> link);
            IConfigurator Link(Expression<Func<T, int>> link);
            IConfigurator Link(Expression<Func<T, string>> link);
            IConfigurator Mvvm<TViewClass>() where TViewClass : IViewClass;
            IConfigurator Mvvm() => Mvvm<IListElementViewClass>();
        }

    }
    public interface IMainListConfigurator<T> : IListConfigurator<T>
        where T : class, IEntity, new()
    {
        IMainListConfigurator<T> Header(object header);
        IMainListConfigurator<T> AddAllowed();
        IMainListConfigurator<T> DeleteAllowed();
        IMainListConfigurator<T> ExportAllowed();
        IMainListConfigurator<T> ImportAllowed();
        IMainListConfigurator<T> StaticFilter(Expression<Func<T,bool>> filter);
    }

    public interface IListConfigurator<T> where T : class, IEntity, new()
    {
        TList Target<TList>() where TList : IEntityListViewModel<T>
        {
            if (Target() is TList list) return list;
            throw new ArgumentException($"{Target().GetType().Name} is not {typeof(TList)}");
        }
        IEntityListViewModel<T> Target();

        public IColumnConfigurator<T> Column();
        public IFilterConfigurator<T, TF> Filter<TF>() where TF : class, IFilterViewModel;
    }


    public interface IColumnConfigurator
    {

    }

    public interface IColumnConfigurator<T> : IListConfigurator<T>, IColumnConfigurator, IDisposable
    where T : class, IEntity, new()
    {
        IColumn<T> CurrentColumn { get; }

        IColumnConfigurator<T> Header(object caption);
        IColumnConfigurator<T> Width(double width);
        IColumnConfigurator<T> Id(string caption);
        IColumnConfigurator<T> OrderBy(Func<T, object> orderBy);
        IColumnConfigurator<T> OrderByOrder(int order);
        public IColumnConfigurator<T> Hidden();

        public IDateTimeColumnConfigurator<T> Link(Expression<Func<T, DateTime?>> link);
        public IForeignNullableColumnConfigurator<T> Link(Expression<Func<T, int?>> link);
        public IForeignNotNullColumnConfigurator<T> Link(Expression<Func<T, int>> link);
        public IStringColumnConfigurator<T> Link(Expression<Func<T, string>> link);
        IColumnConfigurator<T> Mvvm<TViewClass>() where TViewClass : IViewClass;
        IColumnConfigurator<T> Mvvm() => Mvvm<IListElementViewClass>();
    }
    public interface IStringColumnConfigurator<T> : IColumnConfigurator<T>, IDisposable
    where T : class, IEntity, new()
    {
        Expression<Func<T, string>> Getter { get; }
        IFilterConfigurator<T,TextFilter> Filter() => 
            Filter<TextFilter>()
            .Header(CurrentColumn.Header)
            .Link(Getter)
            ;
    }
    public interface IDateTimeColumnConfigurator<T> : IColumnConfigurator<T>, IDisposable
    where T : class, IEntity, new()
    {
        Expression<Func<T, DateTime?>> Getter { get; }
        IFilterConfigurator<T,DateFilter> Filter() => Filter<DateFilter>().Header(CurrentColumn.Header).Link(Getter);
    }

    public interface IForeignNotNullColumnConfigurator<T> : IColumnConfigurator<T>, IDisposable
        where T : class, IEntity, new()
    {
        Expression<Func<T, int>> Getter { get;  }
        IFilterConfigurator<T, IEntityFilterNotNull<TE>> EntityFilter<TE>() where TE : class, IEntity, IListableModel, new();
    }
    public interface IForeignNullableColumnConfigurator<T> : IColumnConfigurator<T>, IDisposable
        where T : class, IEntity, new()
    {
        Expression<Func<T, int?>> Getter { get; }
        IFilterConfigurator<T,IEntityFilterNullable<TE>> EntityFilter<TE>() where TE : class, IEntity, IListableModel, new(); 
    }
    public interface IFilterConfigurator<T> : IListConfigurator<T>, IDisposable
        where T : class, IEntity, new()
    {
    }


    public interface IFilterConfigurator<T, out TF> : IFilterConfigurator<T>
        where T : class, IEntity, new()
    {
        TF CurrentFilter { get; }

        IFilterConfigurator<T, TF> Header(object header);
        IFilterConfigurator<T, TF> IconPath(string path);
//        IFilterConfigurator<T, TF> Link(Func<T,object> link);

    }

    public interface IColumnsProvider<T>
    {
        void Populate(object grid);

        object GetValue(T obj, string name);

        object GetView();
        void AddColumn(IColumn<T> column);
    }
}

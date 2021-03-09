using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HLab.Base.Fluent;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core
{    public interface IColumn<T>
    {
        bool Hidden { set; }
        object Caption { set; }
        double Width { set; }
        Func<T, object> OrderBy { get; set; }
        int OrderByOrder { get;  set; }
        bool OrderDescending { get;  set; }
        void AddGetter(Func<T,object, object> getter);
        string Id { get; set; }
        Func<T, object> Getter { get; set; }
    }

    public interface IColumnConfigurator<T>
    {
        IColumnConfigurator<T> Header(object caption);
        IColumnConfigurator<T> Width(double width);
        IColumnConfigurator<T> Id(string caption);
        IColumnConfigurator<T> OrderBy(Func<T, object> orderBy);
        IColumnConfigurator<T> OrderByOrder(int order);
        public IColumnConfigurator<T> Hidden { get; }
        public IColumnConfigurator<T> Content(Func<T, object> getContent);
        public IColumnConfigurator<T> Content(Func<T, Task<object>> getContent);
        public IColumnConfigurator<T> Column { get; }
        public IColumnConfigurator<T> Filter<TF>()  where TF : IFilterViewModel, new();
        IColumnConfigurator<T> Mvvm<TViewClass>() where TViewClass : IViewClass;
        IColumn<T> Current { get; }
    }

    public interface IColumnsProvider<T>
    {
        void Populate(object grid);

        void Configure(Func<IColumnConfigurator<T>, IColumnConfigurator<T>> f);

        object GetValue(T obj, string name);

        object GetView();
    }
}

using HLab.Erp.Data.Observables;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Wpf.EntityLists
{
    public interface IListElementViewClass : IViewClass {}


    public interface IColumnsProvider<T>
    {
        void Populate(object grid);

        object GetValue(T obj, string name);

        object GetView();

        void AddColumn(IColumn<T> column);

        IObservableQuery<T> List {get;}
    }

}

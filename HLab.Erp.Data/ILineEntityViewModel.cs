using System.ComponentModel;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Data
{
    public interface ILineEntityViewModel<T>
        where T : class, INotifyPropertyChanged, IEntity, new()
    {
        IViewModel<T> ViewModel { get; set; }
        object ParentObject { get; set; }
    }
}
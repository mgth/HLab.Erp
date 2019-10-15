using System;
using System.ComponentModel;
using System.Windows.Input;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Data
{
    public interface IListEntityViewModel<in T, in TLine> : IListEntityViewModel<T>
        where T : class, INotifyPropertyChanged, IEntity, new()
        where TLine : INotifyPropertyChanged, IViewModel<T>, ILineEntityViewModel<T>, new()
    {
        bool Select(TLine vm);
    }

    public interface IListEntityViewModel<in T> : IListEntityViewModel
        where T : class//, new()
    {
        bool Select(T item);
    }

    public interface IListEntityViewModel
    {
        /// <summary>
        /// returns To type for lines
        /// </summary>
        //DbContext DbContextObject { get; }

        Type EntityViewModelType { get; }
        Type LineViewModelType { get; }
        Type EntityType { get; }

        string Search { get; set; }
        object SelectedObjectEntity { get; set; }

        IView Selector { get; }

        ICommand AddCommand { get; }
        ICommand RemoveCommand { get; }

        void UpdateSource();
    }
}
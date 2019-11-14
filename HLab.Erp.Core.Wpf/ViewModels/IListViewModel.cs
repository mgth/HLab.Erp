using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;

namespace HLab.Erp.Core.ViewModels
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    public interface IListViewModel
    {
        void PopulateDataGrid(DataGrid grid);

        void SetOpenAction(Action<object> action);
    }

    public interface IListViewModel<T> where T : class, IEntity
    {
        T Model { get; set; }

        ObservableQuery<T> List { get; }
        ColumnsProvider<T> Columns { get; }

        ObservableCollection<IFilterViewModel> Filters { get; }
    }
}
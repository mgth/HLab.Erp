using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Core.ViewModels;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;

namespace HLab.Erp.Core.EntityLists
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    public interface IEntityListViewModel
    {
        void PopulateDataGrid(DataGrid grid);

        void SetOpenAction(Action<object> action);
    }
    public interface IEntityListViewModel<T> where T : class, IEntity
    {
        T Model { get; set; }

        ObservableQuery<T> List { get; }
        ColumnsProvider<T> Columns { get; }

        ObservableCollection<IFilterViewModel> Filters { get; }
    }

}
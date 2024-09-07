#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using HLab.Base.Fluent;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;

namespace HLab.Erp.Core
{
    public interface IFiltersFluentConfigurator<T> where T : class, IEntity
    {
        IObservableQuery<T> List { get; }
    }

    public interface IFiltersFluentConfigurator<T, out TFilter> : IFluentConfigurator<TFilter>, IFiltersFluentConfigurator<T>
        where TFilter : IFilter, new()
        where T : class, IEntity
    {
    }


    public class FiltersFluentConfigurator<T, TFilter>(IObservableQuery<T> list, TFilter target)
        : FluentConfigurator<TFilter>(target), IFiltersFluentConfigurator<T> //IFiltersFluentConfigurator<T, TFilter>
        where TFilter : IFilter, new()
        where T : class, IEntity
    //        where T : class, IEntity
    {
        public IObservableQuery<T> List { get; } = list;
    }
    /// <summary>
    /// 
    /// 
    /// </summary>
    public interface IEntityListViewModel : INotifyPropertyChanged
    {
        void Populate(object grid);

        void Drop(object source, object target, bool after);

        void SetOpenAction(Action<object> action);
        void SetSelectAction(Action<object?> action);

        bool ShowFilters { get; set; }
        bool ShowMenu { get; set; }
        bool AllowManualOrder { get; set; }

        ReadOnlyObservableCollection<IFilter> Filters { get; }
        string FiltersPresetName {get; set;}
        string FiltersPresetSelected {get; set;}
        ReadOnlyObservableCollection<string> FilterPresets { get; }

        ICommand AddCommand { get; }
        ICommand DeleteCommand { get; }
        ICommand OpenCommand { get; }

        ICommand ImportCommand { get; }
        ICommand ExportCommand { get; }
        ICommand ShowMenuCommand { get; }
        ICommand HideMenuCommand { get; }

        ICommand SaveFiltersPresetCommand { get; }

        dynamic SelectedViewModel { get; set; }
        IEnumerable<int> SelectedIds { get; set; }

        void RefreshColumn(string column);
        void RefreshColumn(string column, int id);

        T GetFilter<T>() where T : IFilter;
        void Start();
        void Stop();

        public bool IsEnabledSimpleAddButton { get; }

        public XElement FiltersToXml();
        public void FiltersFromXml(XElement element);

        public IColumnsProvider Columns { get; }
    }

    public interface IEntityListViewModel<T> : IEntityListViewModel where T : class, IEntity
    {
        IObservableQuery<T> List { get; }
        new IColumnsProvider<T> Columns { get; }
        IColumnsProvider IEntityListViewModel.Columns => Columns;

        void AddFilter(IFilter filter);
    }

}
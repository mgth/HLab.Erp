using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Xml.Linq;
using System.Xml.Serialization;

using HLab.Base.Fluent;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;

namespace HLab.Erp.Core
{
    public interface IFiltersFluentConfigurator<T>
    {
        IObservableQuery<T> List { get; }
    }

    public interface IFiltersFluentConfigurator<T, out TFilter> : IFluentConfigurator<TFilter>, IFiltersFluentConfigurator<T>
        where TFilter : IFilter, new()
        where T : class, IEntity
    {
    }


    public class FiltersFluentConfigurator<T, TFilter> : FluentConfigurator<TFilter>, IFiltersFluentConfigurator<T>//IFiltersFluentConfigurator<T, TFilter>
        where TFilter : IFilter, new()
        //        where T : class, IEntity
    {
        public IObservableQuery<T> List { get; }
        public FiltersFluentConfigurator(IObservableQuery<T> list, TFilter target) : base(target)
        {
            List = list;
        }
    }
    /// <summary>
    /// 
    /// 
    /// </summary>
    public interface IEntityListViewModel : INotifyPropertyChanged
    {
        void Populate(object grid);

        void SetOpenAction(Action<object> action);
        void SetSelectAction(Action<object> action);
        ReadOnlyObservableCollection<IFilter> Filters { get; }
        string FiltersPresetName {get; set;}
        string FiltersPresetSelected {get; set;}
        ReadOnlyObservableCollection<string> FilterPresets { get; }

        ICommand AddCommand { get; }
        ICommand DeleteCommand { get; }
        ICommand OpenCommand { get; }

        ICommand ImportCommand { get; }
        ICommand ExportCommand { get; }

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

    public interface IEntityListViewModel<T> : IEntityListViewModel
    {
        IObservableQuery<T> List { get; }
        new IColumnsProvider<T> Columns { get; }
        IColumnsProvider IEntityListViewModel.Columns => Columns;

        void AddFilter(IFilter filter);
    }

}
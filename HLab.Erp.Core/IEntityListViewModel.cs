using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HLab.Base.Fluent;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;

namespace HLab.Erp.Core
{
    public interface IFiltersFluentConfigurator<T>
    {
        IObservableQuery<T> List { get; }
    }

    public interface IFiltersFluentConfigurator<T,out TFilter> : IFluentConfigurator<TFilter>, IFiltersFluentConfigurator<T>
        where TFilter : IFilter, new()
        where T : class, IEntity
    {
    }


    public class FiltersFluentConfigurator<T,TFilter> : FluentConfigurator<TFilter>, IFiltersFluentConfigurator<T>//IFiltersFluentConfigurator<T, TFilter>
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
    public interface IEntityListViewModel
    {
        void Populate(object grid);

        void SetOpenAction(Action<object> action);
        void SetSelectAction(Action<object> action);
        ReadOnlyObservableCollection<IFilter> Filters { get; }

        ICommand AddCommand { get; }
        ICommand DeleteCommand { get; }
        ICommand OpenCommand { get; }

        dynamic SelectedViewModel { get; set; }
        IEnumerable<int> SelectedIds { get; set; }

        void RefreshColumn(string column);
        void RefreshColumn(string column, int id);

        T GetFilter<T>() where T : IFilter;
        void Start();
        void Stop();
    }

    public interface IEntityListViewModel<T> : IEntityListViewModel
    {
        IObservableQuery<T> List { get; }
        IColumnsProvider<T> Columns { get; }

        void AddFilter(IFilter filter);
    }

}
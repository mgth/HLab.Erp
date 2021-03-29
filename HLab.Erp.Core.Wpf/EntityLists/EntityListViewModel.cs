using HLab.Base.Extensions;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core.Tools.Details;
using HLab.Erp.Core.ViewModels.EntityLists;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using HLab.Mvvm.Application;

namespace HLab.Erp.Core.EntityLists
{
    public abstract class EntityListViewModel : ViewModel, IEntityListViewModel
    {
        protected EntityListViewModel()
        {
            Filters = new(_filters);
            H<EntityListViewModel>.Initialize(this);
        }

        public abstract void Populate(object grid);

        public abstract void SetOpenAction(Action<object> action);
        public abstract void SetSelectAction(Action<object> action);
        protected readonly ObservableCollection<IFilterViewModel> _filters = new ();
        public ReadOnlyObservableCollection<IFilterViewModel> Filters { get; }

        [Import] private Func<Type, object> _get;

        public T Filter<T>(Action<T> configure = null) where T : IFilterViewModel
        {
            var filter = (T)_get(typeof(T));
            configure?.Invoke(filter);
            _filters.Add(filter);
            return filter;
        }

        public abstract dynamic SelectedViewModel { get; set; }
        public abstract IEnumerable<int> SelectedIds { get; set; }
        public abstract void RefreshColumn(string column);
        public abstract void RefreshColumn(string column, int id);
        protected abstract Task AddEntityAsync();

        public abstract ICommand AddCommand { get; }
        public abstract ICommand DeleteCommand { get; }
    }

    public abstract class EntityListViewModel<T> : EntityListViewModel, IEntityListViewModel<T>
        where T : class, IEntity, new()
    {
        public virtual string Title => _title.Get();
        private readonly IProperty<string> _title = H<EntityListViewModel<T>>.Property<string>(c => c
            .Set(e => "{" + e.GetTitle() + "}")
        );

        public string IconPath => "Icons/Entities/" + typeof(T).Name;

        private string GetTitle() => GetType().Name.BeforeSuffix("ListViewModel").FromCamelCase();

        
        [Import] protected IDocumentService _docs;
        [Import] protected IMessageBus _msg;
        [Import] protected IDataService _data;

        
        [Import] public ObservableQuery<T> List { get; }
        public IColumnsProvider<T> Columns { get; }


        public IEntityListViewModel<T> AddFilter<TFilter>(Action<FiltersFluentConfigurator<T,TFilter>> configure)
            where TFilter : IFilterViewModel, new()
        {
            var c = new FiltersFluentConfigurator<T,TFilter>(List,new TFilter());
            configure(c);
            _filters.Add(c.Target);
            return this;
        }

        public override void RefreshColumn(string column)
        {
            foreach (var vm in ListViewModel)
            {
                vm.Refresh(column);
            }
        }
        public override void RefreshColumn(string column, int id)
        {
            foreach (var vm in ListViewModel.Where(e => e.Id == id))
            {
                vm.Refresh(column);
            }
        }

        public ObservableCollection<IObjectMapper> ListViewModel { get; } = new ();
        private readonly ConcurrentDictionary<T,dynamic> _cache = new ();

        [Import] private Func<ObservableQuery<T>,ColumnsProvider<T>> _getColumnsProvider;

        protected EntityListViewModel()
        {
            Columns = _getColumnsProvider(List);
            List_CollectionChanged(null,new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Add,List,0));
            List.CollectionChanged += List_CollectionChanged;

            OpenAction = target => _docs.OpenDocumentAsync(target);
            
            H<EntityListViewModel<T>>.Initialize(this);

        }

        public ICommand OpenCommand { get; } = H<EntityListViewModel<T>>.Command(c => c
            .CanExecute(e=>e.Selected!=null)
            .Action(e => e.OpenAction.Invoke(e.Selected))
            .On(e => e.Selected).CheckCanExecute()
        );

        public ICommand RefreshCommand { get; } = H<EntityListViewModel<T>>.Command(c => c
            .Action( e => e.List.Update())
        );

        protected Action<T> OpenAction;
        public override void SetOpenAction(Action<object> action) => OpenAction = action;
        public void SetOpenAction(Action<T> action) => OpenAction = action;

        protected Action<T> SelectAction;
        public override void SetSelectAction(Action<object> action) => SelectAction = action;
        public void SetSelectAction(Action<T> action) => SelectAction = action;

        [Import] protected Func<T> CreateInstance;
        protected override Task AddEntityAsync()
        {
            var entity = CreateInstance();

            ConfigureEntity(entity);

            return _docs.OpenDocumentAsync(entity);
        }

        protected virtual void ConfigureEntity(T entity) { }
        public string Message 
        {
            get => _message.Get(); 
            set => _message.Set(value);
        }
        private readonly IProperty<string> _message = H<EntityListViewModel<T>>.Property<string>();

        public bool AddAllowed 
        {
            get => _addAllowed.Get(); 
            set => _addAllowed.Set(value);
        }
        private readonly IProperty<bool> _addAllowed = H<EntityListViewModel<T>>.Property<bool>();

        public bool DeleteAllowed 
        {
            get => _deleteAllowed.Get(); 
            set => _deleteAllowed.Set(value);
        }
        private readonly IProperty<bool> _deleteAllowed = H<EntityListViewModel<T>>.Property<bool>();
        public bool OpenAllowed 
        {
            get => _openAllowed.Get(); 
            set => _openAllowed.Set(value);
        }
        private readonly IProperty<bool> _openAllowed = H<EntityListViewModel<T>>.Property<bool>();


        private void List_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newIndex = e.NewStartingIndex;
                    foreach (var n in e.NewItems!.OfType<T>())
                    {
                        ObjectMapper<T> om = _cache.GetOrAdd(n,o => new ObjectMapper<T>(o, Columns));
                        ListViewModel.Insert(newIndex++,om);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems!.OfType<T>())
                    {
                        //TODO should keep deleted elements in cache for some times
                        if (_cache.TryRemove(item, out var m))
                        {
                            ListViewModel.Remove(m);
                        }
                        else
                        {
                            var mapper = ListViewModel.FirstOrDefault(x => (x is ObjectMapper<T> om) && Equals(om.Model.Id,item.Id));

                            if (mapper != null)
                                ListViewModel.Remove(mapper);
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                    ListViewModel.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //public GridView View => _view.Get();
        //private readonly IProperty<GridView> _view = H<EntityListViewModel<T>>.Property<GridView>(c => c
        //    .Set(e => e.Columns.GetView() as GridView));

        public ListCollectionView ListCollectionView => _listCollectionView.Get();
        private readonly IProperty<ListCollectionView> _listCollectionView = H<EntityListViewModel<T>>.Property<ListCollectionView>(c => c
            .Set(setter: e =>
            {
                var lcv = new ListCollectionView(e.ListViewModel);
                lcv.GroupDescriptions.Add(new PropertyGroupDescription("FileId"));
                return lcv;
            }));

        public override void Populate(object grid)
        {
            if (grid is ItemsControl dataGrid)
            {
                dataGrid.SourceUpdated += delegate(object sender, DataTransferEventArgs args)
                {
                    ICollectionView cv = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
                    if (cv != null)
                    {
                        cv.GroupDescriptions.Clear();
                        cv.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
                    }
                };

                Columns.Populate(grid);
            }
        }

        public T Selected
        {
            get => _selected.Get();
            set {
                if (_selected.Set(value))
                {
                    if (SelectAction != null)
                        SelectAction(value);
                    else
                        _msg.Publish(new DetailMessage(value));
                }
            }
        }

        private readonly IProperty<T> _selected = H<EntityListViewModel<T>>.Property<T>();

        public override dynamic SelectedViewModel
        {
            get => _selectedViewModel.Get();
            set => Selected = value?.Model;
        }

        private readonly IProperty<dynamic> _selectedViewModel = H<EntityListViewModel<T>>.Property<dynamic>(c => c
            .Set(e => e.Selected==null?null:e._cache.GetOrAdd(e.Selected, o => new ObjectMapper<T>(o, e.Columns)))
            .On(e => e.Selected)
        .Update()
        );

        public override IEnumerable<int> SelectedIds
        {
            get => _selectedIds.Get();
            set => _selectedIds.Set(value);
        }
        private readonly IProperty<IEnumerable<int>> _selectedIds = H<EntityListViewModel<T>>.Property<IEnumerable<int>>();

        public override ICommand DeleteCommand { get; } = H<EntityListViewModel<T>>.Command(c => c
                .CanExecute(e => e.CanExecuteDelete())
                .Action(async e => await e.DeleteEntityAsync(e.Selected))
                .On(e => e.Selected)
                .CheckCanExecute()
        );

        public override ICommand AddCommand { get; } = H<EntityListViewModel<T>>.Command(c => c
                .CanExecute(e => e.CanExecuteAdd())
                .Action(async e => await e.AddEntityAsync())
                .On(e => e.Selected)
                .CheckCanExecute()
        );

        protected async Task DeleteEntityAsync(T entity)
        {
            await _docs.CloseDocumentAsync(entity);
            try
            {
                if (await _data.DeleteAsync(entity))
                {
                    List.Update();
                }
                Message = null;
            }
            catch(Exception ex)
            {
                Message = ex.Message;
                while(ex.InnerException != null) {
                    ex = ex.InnerException;
                    Message = "\n" + ex.Message;
                }
            }
        }

        protected virtual bool CanExecuteDelete() => false;
        protected virtual bool CanExecuteAdd() => true;
        protected virtual bool CanExecuteOpen() => true;
    }
}

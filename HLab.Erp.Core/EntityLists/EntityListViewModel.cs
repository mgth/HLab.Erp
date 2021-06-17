using HLab.Base.Extensions;
using HLab.Erp.Core.ViewModels.EntityLists;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NPoco;
using HLab.Erp.Core.Tools.Details;
using System.ComponentModel;
using HLab.Erp.Core.ListFilterConfigurators;

namespace HLab.Erp.Core.EntityLists
{
    public interface IEntityListHelper
    {
        object GetListView(IList list);
    }

    public interface IEntityListHelper<T> : IEntityListHelper where T : class, IEntity, new()
    {
        void Populate(object grid, IColumnsProvider<T> provider);
        Task ExportAsync(IObservableQuery<T> list, IContractResolver resolver);
        public Task<IEnumerable<T>> ImportAsync();
    }

    public abstract class EntityListViewModel : ViewModel, IEntityListViewModel
    {
        protected Func<Type, object> _get;
        protected IErpServices Erp { get; private set; }

        public void Inject(IErpServices erp, Func<Type, object> get)
        {
            _get = get;
            Erp = erp;
            Filters = new(filters);
            H<EntityListViewModel>.Initialize(this);
        }

        public abstract void Populate(object grid);

        public abstract void SetOpenAction(Action<object> action);
        public abstract void SetSelectAction(Action<object> action);
        protected readonly ObservableCollection<IFilter> filters = new();
        public ReadOnlyObservableCollection<IFilter> Filters { get; private set; }


        public void AddFilter(IFilter filter) => filters.Add(filter);
        public T GetFilter<T>() where T : IFilter
        {
            var locate = (Func<IEntityListViewModel,T>)_get(typeof(Func<IEntityListViewModel,T>));
            return locate(this);
        }

        public abstract void Start();
        public abstract void Stop();


        public abstract dynamic SelectedViewModel { get; set; }
        public abstract IEnumerable<int> SelectedIds { get; set; }
        public abstract void RefreshColumn(string column);
        public abstract void RefreshColumn(string column, int id);
        protected abstract Task AddEntityAsync();

        public abstract ICommand AddCommand { get; }
        public abstract ICommand DeleteCommand { get; }
        public abstract ICommand OpenCommand { get; }

        public string ErrorMessage => string.Join(Environment.NewLine,_errors.OrderBy(e => e.Item1).Select(e => e.Item2));

        private List<Tuple<string,string>> _errors = new List<Tuple<string, string>>();

        public void AddErrorMessage(string key,string message)
        {
            _errors.Add(new(key,message));
            ClassHelper.OnPropertyChanged(new PropertyChangedEventArgs(nameof(ErrorMessage)));
        }

        public void RemoveErrorMessage(string key)
        {
            _errors.RemoveAll(e => e.Item1 == key);
            ClassHelper.OnPropertyChanged(new PropertyChangedEventArgs(nameof(ErrorMessage)));
        }
    }

    public abstract class EntityListViewModel<T> : EntityListViewModel, IEntityListViewModel<T>
        where T : class, IEntity, new()
    {
        private IEntityListHelper<T> _helper;

        public object Header
        {
            get => _header.Get();
            set => _header.Set(value);
        }
        private readonly IProperty<object> _header = H<EntityListViewModel<T>>.Property<object>();

        public string IconPath
        {
            get => _iconPath.Get();
            set => _iconPath.Set(value);
        }
        private readonly IProperty<string> _iconPath = H<EntityListViewModel<T>>.Property<string>();

        private string GetTitle() => $"{{{GetName().FromCamelCase()}}}";
        private string GetName() => GetType().Name.BeforeSuffix("ListViewModel");

        protected Func<T> CreateInstance { get; private set; }

        public IObservableQuery<T> List { get; private set; }

        public IColumnsProvider<T> Columns { get; set; }


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

        public ObservableCollection<IObjectMapper> ListViewModel { get; } = new();
        private readonly ConcurrentDictionary<T, dynamic> _cache = new();


        public void Inject(
            IEntityListHelper<T> helper,
            Func<T> createInstance,
            IColumnsProvider<T> columnsProvider
            )
        {
            _helper = helper;
            CreateInstance = createInstance;

            Columns = columnsProvider;
            List = columnsProvider.List;


            List_CollectionChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, List, 0));
            List.CollectionChanged += List_CollectionChanged;


            H<EntityListViewModel<T>>.Initialize(this);

            _configurator.Invoke(new ColumnConfigurator<T,object,IFilter<object>>(this,Erp))?.Dispose();

            Header ??= GetTitle();
            OpenAction ??= target => Erp.Docs.OpenDocumentAsync(target);
            IconPath ??= "Icons/Entities/" + typeof(T).Name;
        }

        private readonly Func<IColumnConfigurator<T,object,IFilter<object>>, IDisposable> _configurator;
        protected EntityListViewModel(Func<IColumnConfigurator<T,object,IFilter<object>>, IDisposable> configurator)
        {
            _configurator = configurator;
        }

        #region COMMANDS
        public override ICommand OpenCommand { get; } = H<EntityListViewModel<T>>.Command(c => c
            .CanExecute((e, a) =>
            {
                e.RemoveErrorMessage("Open");
                if(a is T target)
                    return e.CanExecuteOpen(target,m=>e.AddErrorMessage("Open",m));
                return false;
            })
            .Action((e,a) =>
            {
                if(a is T entity)
                    e.OpenAction?.Invoke(entity);
            })
            .On(e => e.Selected).CheckCanExecute(e => e.Selected)
        );



        public ICommand RefreshCommand { get; } = H<EntityListViewModel<T>>.Command(c => c
            .Action(e => e.List.Update())
        );
        
        public override ICommand DeleteCommand { get; } = H<EntityListViewModel<T>>.Command(c => c
            .CanExecute((e, a) =>
            {
                e.RemoveErrorMessage("Delete");
                if(a is T target)
                {
                    return e.CanExecuteDelete(target,m=>e.AddErrorMessage("Delete",m));
                }
                return e.CanExecuteDelete(null,m=>e.AddErrorMessage("Delete",m));
            })
            .Action(async e => await e.DeleteEntityAsync(e.Selected))
            .On(e => e.Selected)
            .CheckCanExecute(e => e.Selected)
        );

        public override ICommand AddCommand { get; } = H<EntityListViewModel<T>>.Command(c => c
            .CanExecute(e =>
            {
                e.RemoveErrorMessage("Add");
                return e.CanExecuteAdd(m => e.AddErrorMessage("Add", m));
            })
            .Action(async e => await e.AddEntityAsync())
            .On(e => e.Selected)
            .CheckCanExecute()
        );
        public ICommand ExportCommand { get; } = H<EntityListViewModel<T>>.Command(c => c
            .CanExecute(e =>
            {
                e.RemoveErrorMessage("Export");
                return e.CanExecuteExport(m => e.AddErrorMessage("Export", m));
            })
            .Action(async e => await e.ExportAsync())
            .On(e => e.Selected)
            .CheckCanExecute()
        );
        public ICommand ImportCommand { get; } = H<EntityListViewModel<T>>.Command(c => c
            .CanExecute(e =>
            {
                e.RemoveErrorMessage("Import");
                return e.CanExecuteImport(m => e.AddErrorMessage("Import", m));
            })
            .Action(async e => await e.ImportAsync())
            .On(e => e.Selected)
            .CheckCanExecute()
        );
        protected virtual bool CanExecuteOpen(T arg,Action<string> errorAction) => true;

        protected virtual bool CanExecuteDelete(T arg,Action<string> errorAction) => false;
        protected virtual bool CanExecuteAdd(Action<string> errorAction) => false;

        protected virtual bool CanExecuteImport(Action<string> errorAction) => false;
        protected virtual bool CanExecuteExport(Action<string> errorAction) => false;
        #endregion



        protected Action<T> OpenAction;
        public override void SetOpenAction(Action<object> action) => OpenAction = action;
        public void SetOpenAction(Action<T> action) => OpenAction = action;

        protected Action<T> SelectAction;
        public override void SetSelectAction(Action<object> action) => SelectAction = action;
        public void SetSelectAction(Action<T> action) => SelectAction = action;

        protected override Task AddEntityAsync()
        {
            var entity = CreateInstance();

            ConfigureEntity(entity);

            return Erp.Docs.OpenDocumentAsync(entity);
        }

        protected virtual void ConfigureEntity(T entity) { }
        public string Message
        {
            get => _message.Get();
            set => _message.Set(value);
        }
        private readonly IProperty<string> _message = H<EntityListViewModel<T>>.Property<string>();


        private void List_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newIndex = e.NewStartingIndex;
                    foreach (var n in e.NewItems!.OfType<T>())
                    {
                        ObjectMapper<T> om = _cache.GetOrAdd(n, o => new ObjectMapper<T>(o, Columns));
                        ListViewModel.Insert(newIndex++, om);
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
                            var mapper = ListViewModel.FirstOrDefault(x => (x is ObjectMapper<T> om) && Equals(om.Model.Id, item.Id));

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

        public object ListCollectionView => _listCollectionView.Get();
        private readonly IProperty<object> _listCollectionView = H<EntityListViewModel<T>>.Property<object>(c => c
            .Set(setter: e => e._helper.GetListView(e.ListViewModel)));

        public override void Populate(object grid) => _helper.Populate(grid, Columns);

        public T Selected
        {
            get => _selected.Get();
            set
            {
                if (_selected.Set(value))
                {
                    if (SelectAction != null)
                        SelectAction(value);
                    else
                        Erp.Message.Publish(new DetailMessage(value));
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
            .Set(e => e.Selected == null ? null : e._cache.GetOrAdd(e.Selected, o => new ObjectMapper<T>(o, e.Columns)))
            .On(e => e.Selected)
            .Update()
        );

        public override IEnumerable<int> SelectedIds
        {
            get => _selectedIds.Get();
            set => _selectedIds.Set(value);
        }
        private readonly IProperty<IEnumerable<int>> _selectedIds = H<EntityListViewModel<T>>.Property<IEnumerable<int>>();


        private class ExportIdValueProvider : IValueProvider
        {
            private readonly IValueProvider _foreignProvider;

            public ExportIdValueProvider(IValueProvider foreignProvider)
            {
                _foreignProvider = foreignProvider;
            }

            public void SetValue(object target, object? value)
            {
                throw new NotImplementedException();
            }

            public object? GetValue(object target)
            {
                var value = _foreignProvider.GetValue(target);
                if (value is IEntityWithExportId v) return v.ExportId;
                return null;
            }
        }
        private class ContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var properties = base.CreateProperties(type, memberSerialization);

                List<JsonProperty> outputList = new();

                foreach (var p in properties)
                {
                    if (!p.Writable) continue;
                    if (p.PropertyType == null) continue;
                    if (p.PropertyType == typeof(string))
                    {
                        if (p.AttributeProvider.GetAttributes(true).OfType<IgnoreAttribute>().Any()) continue;
                        outputList.Add(p);
                        continue;
                    }
                    if (p.PropertyType.IsClass)
                    {
                        if (typeof(IEntityWithExportId).IsAssignableFrom(p.PropertyType))
                        {
                            p.ValueProvider = new ExportIdValueProvider(p.ValueProvider);
                            outputList.Add(p);
                            continue;
                        }
                    }
                    if (p.AttributeProvider.GetAttributes(true).OfType<IgnoreAttribute>().Any()) continue;

                    if (p.PropertyType.IsInterface) continue;

                    outputList.Add(p);
                }


                return outputList;
            }
        }


        private Task ExportAsync() => _helper.ExportAsync(List, new ContractResolver());

        private async Task ImportAsync()
        {
            var list = await _helper.ImportAsync();
            foreach (var entity in list)
            {
                await ImportAsync(Erp.Data, entity);
            }
        }

        protected virtual async Task ImportAsync(IDataService data, T newValue)
        { }

        protected async Task DeleteEntityAsync(T entity)
        {
            await Erp.Docs.CloseDocumentAsync(entity);
            try
            {
                if (await Erp.Data.DeleteAsync(entity))
                {
                    List.Update();
                }
                Message = null;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    Message = "\n" + ex.Message;
                }
            }
        }


        public override void Start()
        {
            List.Update();
            List.Start();
        }
        public override void Stop()
        {
            List.Stop();
        }


        void IEntityListViewModel<T>.AddFilter(IFilter filter) => AddFilter(filter);
        IObservableQuery<T> IEntityListViewModel<T>.List => List;
        IColumnsProvider<T> IEntityListViewModel<T>.Columns => Columns;
    }
}

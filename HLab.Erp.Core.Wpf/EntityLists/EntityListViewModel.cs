using HLab.Base.Extensions;
using HLab.Core.Annotations;
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
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using HLab.Erp.Core.Annotations;
using HLab.Mvvm.Application;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NPoco;
using Grace.DependencyInjection.Attributes;

namespace HLab.Erp.Core.EntityLists
{
    public abstract class EntityListViewModel : ViewModel, IEntityListViewModel
    {
        protected Func<Type, object> _get;
        protected IErpServices Erp { get; private set; }

        bool _injected;

        [Import]
        public void Inject(IErpServices erp, Func<Type, object> get)
        {
            if (_injected) return;
            _injected = true;

            _get = get;
            Erp = erp;
            Filters = new(filters);
            H<EntityListViewModel>.Initialize(this);
        }

        public abstract void Populate(object grid);

        public abstract void SetOpenAction(Action<object> action);
        public abstract void SetSelectAction(Action<object> action);
        protected readonly ObservableCollection<IFilterViewModel> filters = new();
        public ReadOnlyObservableCollection<IFilterViewModel> Filters { get; private set; }


        public void AddFilter(IFilterViewModel filter) => filters.Add(filter);
        public T GetFilter<T>() where T : IFilterViewModel => (T)_get(typeof(T));
        public abstract void Start();


        public abstract dynamic SelectedViewModel { get; set; }
        public abstract IEnumerable<int> SelectedIds { get; set; }
        public abstract void RefreshColumn(string column);
        public abstract void RefreshColumn(string column, int id);
        protected abstract Task AddEntityAsync();

        public abstract ICommand AddCommand { get; }
        public abstract ICommand DeleteCommand { get; }

    }

    public class EntityListViewModel<T> : EntityListViewModel, IEntityListViewModel<T>
        where T : class, IEntity, new()
    {
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

        public ICommand MenuCommand { get; } = H<EntityListViewModel<T>>.Command(c => c
            .Action(e => e.Erp.Docs.OpenDocumentAsync(e.GetType()))
        );


        protected Func<T> CreateInstance { get; private set; }


        public ObservableQuery<T> List { get; private set; }

        public IColumnsProvider<T> Columns { get; set; }


        public IEntityListViewModel<T> AddFilter<TFilter>(Action<FiltersFluentConfigurator<T, TFilter>> configure)
            where TFilter : IFilterViewModel, new()
        {
            var c = new FiltersFluentConfigurator<T, TFilter>(List, new TFilter());
            configure(c);
            filters.Add(c.Target);
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

        public ObservableCollection<IObjectMapper> ListViewModel { get; } = new();
        private readonly ConcurrentDictionary<T, dynamic> _cache = new();

        private Func<ObservableQuery<T>, ColumnsProvider<T>> _getColumnsProvider;

        bool _injected = false;

        [Import]
        public void Inject(
            Func<T> createInstance,
            ObservableQuery<T> list,
            Func<ObservableQuery<T>, ColumnsProvider<T>> getColumnsProvider
            )
        {
            if (_injected) return;
            _injected = true;

            CreateInstance = createInstance;
            List = list;

            _getColumnsProvider = getColumnsProvider;
            Columns = _getColumnsProvider(List);

            List_CollectionChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, List, 0));
            List.CollectionChanged += List_CollectionChanged;

            OpenAction = target => Erp.Docs.OpenDocumentAsync(target);

            H<EntityListViewModel<T>>.Initialize(this);

//            using (var s = List.Suspender.Get())
            using (var c = new ListConfigurator<T>(this))
            {
                _configurator.Invoke(c);
            }

            Header ??= GetTitle();

            if (string.IsNullOrWhiteSpace(IconPath))
                IconPath = "Icons/Entities/" + typeof(T).Name;
        }

        private readonly Func<IMainListConfigurator<T>, IListConfigurator<T>> _configurator;
        public EntityListViewModel(Func<IMainListConfigurator<T>, IListConfigurator<T>> configurator)
        {
            _configurator = configurator;
        }

        public ICommand OpenCommand { get; } = H<EntityListViewModel<T>>.Command(c => c
            .CanExecute(e => e.Selected != null)
            .Action(e => e.OpenAction.Invoke(e.Selected))
            .On(e => e.Selected).CheckCanExecute()
        );

        public ICommand RefreshCommand { get; } = H<EntityListViewModel<T>>.Command(c => c
            .Action(e => e.List.Update())
        );

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
        public bool ImportAllowed
        {
            get => _importAllowed.Get();
            set => _importAllowed.Set(value);
        }
        private readonly IProperty<bool> _importAllowed = H<EntityListViewModel<T>>.Property<bool>();

        public bool ExportAllowed
        {
            get => _exportAllowed.Get();
            set => _exportAllowed.Set(value);
        }
        private readonly IProperty<bool> _exportAllowed = H<EntityListViewModel<T>>.Property<bool>();
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
                dataGrid.SourceUpdated += delegate (object sender, DataTransferEventArgs args)
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
        public ICommand ExportCommand { get; } = H<EntityListViewModel<T>>.Command(c => c
                .CanExecute(e => e.CanExecuteAdd())
                .Action(async e => await e.ExportAsync())
                .On(e => e.Selected)
                .CheckCanExecute()
        );

        private class ExportIdValueProvider : IValueProvider
        {
            private IValueProvider _foreignProvider;

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


        private async Task ExportAsync()
        {
            var filename = typeof(T).Name + "-export.gz";
            SaveFileDialog saveFileDialog = new() { FileName = filename, DefaultExt = "gz" };
            if (saveFileDialog.ShowDialog() == false) return;

            var text = JsonConvert.SerializeObject(
                List.ToList(),
                Formatting.Indented,
                new JsonSerializerSettings { ContractResolver = new ContractResolver() });

            await using var sourceStream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            await using var fileStream = File.Create(saveFileDialog.FileName);
            await using var gzipStream = new GZipStream(fileStream, CompressionMode.Compress);
            try
            {
                await sourceStream.CopyToAsync(gzipStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public ICommand ImportCommand { get; } = H<EntityListViewModel<T>>.Command(c => c
                .CanExecute(e => e.CanExecuteAdd())
                .Action(async e => await e.ImportAsync())
                .On(e => e.Selected)
                .CheckCanExecute()
        );

        private async Task ImportAsync()
        {
            var filename = typeof(T).Name + "-export.gz";
            OpenFileDialog openFileDialog = new() { FileName = filename, Filter = $"{typeof(T).Name}|*.{typeof(T).Name}.gz" };
            if (openFileDialog.ShowDialog() == false) return;

            await using var fileStream = File.OpenRead(openFileDialog.FileName);
            await using var resultStream = new MemoryStream();
            await using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            try
            {
                await gzipStream.CopyToAsync(resultStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }



            var text = Encoding.UTF8.GetString(resultStream.ToArray()); ;
            List<T> list = JsonConvert.DeserializeObject<List<T>>(text);
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

        protected virtual bool CanExecuteDelete() => false;
        protected virtual bool CanExecuteAdd() => true;
        protected virtual bool CanExecuteOpen() => true;

        public override void Start() => List.Start();
    }
}

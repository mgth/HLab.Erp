using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Formats.Asn1;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using System.Xml.Schema;
using HLab.Base.Extensions;
using HLab.Core.Annotations;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.Tools.Details;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using HLab.Options;
using Newtonsoft.Json.Serialization;

namespace HLab.Erp.Core.EntityLists
{
    public abstract class EntityListViewModel : ViewModel
    {
        public virtual Injector Injected { get; }

        public class Injector
        {
            public IOptionsService Options { get; }
            internal Func<Type, object> Get;

            protected Injector(Injector injector) : this(injector.Options, injector.Get){}

            public Injector(IOptionsService options, Func<Type, object> get)
            {
                Options = options;
                Get = get;
            }
        }

        protected EntityListViewModel(Injector injector)
        {
            Injected = injector;
            Filters = new ReadOnlyObservableCollection<IFilter>(_filters);

            H<EntityListViewModel>.Initialize(this);

            PopulatePresets();

            FilterPresets = new ReadOnlyObservableCollection<string>(_filterPresets);
        }

        void PopulatePresets()
        {
            _filterPresets.Clear();
            var list = Injected.Options.GetOptions($"Filters", GetType().Name, null);
            foreach(var item in list) _filterPresets.Add(item);
        }

        public abstract void Populate(object grid);

        public abstract void SetOpenAction(Action<object> action);
        public abstract void SetSelectAction(Action<object> action);
        protected readonly ObservableCollection<IFilter> _filters = new();
        public ReadOnlyObservableCollection<IFilter> Filters { get; }


        public void AddFilter(IFilter filter) => _filters.Add(filter);
        public T GetFilter<T>() where T : IFilter
        {
            var locate = (Func<IEntityListViewModel,T>)Injected.Get(typeof(Func<IEntityListViewModel,T>));
            return locate((IEntityListViewModel)this);
        }

        public abstract void Start();
        public abstract void Stop();
        


        public abstract dynamic SelectedViewModel { get; set; }
        public abstract IEnumerable<int> SelectedIds { get; set; }
        public abstract void RefreshColumn(string column);
        public abstract void RefreshColumn(string column, int id);

        protected abstract Task AddEntityAsync(object arg);
        protected abstract Task AddEntityAsync();

        public abstract ICommand AddCommand { get; }
        public abstract ICommand DeleteCommand { get; }
        public abstract ICommand OpenCommand { get; }
        public abstract ICommand ImportCommand { get; }
        public abstract ICommand ExportCommand { get; }

        public ICommand SaveFiltersPresetCommand {get; } = H<EntityListViewModel>.Command(c => c
            .CanExecute((e, a) => !string.IsNullOrEmpty(e.FiltersPresetName))
            .Action((e,a) =>
            {
                e.SaveFilters();
                e.PopulatePresets();
            })
            .On(e => e.FiltersPresetName).CheckCanExecute()
        );

        void SaveFilters()
        {
            var doc = new XDocument();

            doc.Add(FiltersToXml());

            var xml = doc.ToString();

            Injected.Options.SetValue($"Filters\\{GetType().Name}",FiltersPresetName, xml);
        }



        public string FiltersPresetName
        {
            get => _filtersPresetName.Get();
            set => _filtersPresetName.Set(value);
        }

        readonly IProperty<string> _filtersPresetName = H<EntityListViewModel>.Property<string>();

        public string FiltersPresetSelected
        {
            get => _filtersPresetSelected.Get();
            set
            {
                if (!_filtersPresetSelected.Set(value) || value == null) return;

                var xml = Injected.Options.GetValue($"Filters\\{GetType().Name}",FiltersPresetSelected,null, ()=>"");

                if (string.IsNullOrWhiteSpace(xml)) return;
                
                var doc = XDocument.Parse(xml);

                foreach(var item in doc.Elements())
                    FiltersFromXml(item);
            }
        }

        readonly IProperty<string> _filtersPresetSelected = H<EntityListViewModel>.Property<string>();

        /// <summary>
        /// Type of entity to be passed as argument when adding new entity to list
        /// </summary>
        public virtual Type AddArgumentClass => null;

        public bool IsEnabledSimpleAddButton => AddArgumentClass == null;

        public string ErrorMessage => string.Join(Environment.NewLine,_errors.OrderBy(e => e.Item1).Select(e => e.Item2));


        readonly ObservableCollection<string> _filterPresets = new();
        public ReadOnlyObservableCollection<string> FilterPresets {get;}


        readonly List<Tuple<string,string>> _errors = new();


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

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void  FiltersFromXml(XElement element)
        {
            var filters = Filters.ToList();

            if(element.Name=="Filters")
            {
                foreach(var child in element.Elements())
                {
                        var filter = filters.FirstOrDefault(f => f.Name==child.Name);
                        if (filter == null) continue;

                        filters.Remove(filter);
                        filter.FromXml(child);
                        filter.Enabled = true;
                }
            }

            foreach(var filter in filters) filter.Enabled = false;
        }


        public XElement FiltersToXml()
        {
            var xFilters = new XElement("Filters");

            foreach(var filter in Filters)
            {
                if (!filter.Enabled) continue;

                var xFilter = filter.ToXml();
                xFilters.Add(xFilter);
            }

            return xFilters;
        }
    }

    public abstract partial class EntityListViewModel<T> : EntityListViewModel, IEntityListViewModel<T>, IEntityListViewModel
        where T : class, IEntity, new()
    {

        public object Header
        {
            get => _header.Get();
            set => _header.Set(value);
        }

        readonly IProperty<object> _header = H<EntityListViewModel<T>>.Property<object>();

        public string IconPath
        {
            get => _iconPath.Get();
            set => _iconPath.Set(value);
        }

        readonly IProperty<string> _iconPath = H<EntityListViewModel<T>>.Property<string>();

        string GetTitle() => $"{{{GetName().FromCamelCase()}}}";
        string GetName() => GetType().Name.BeforeSuffix("ListViewModel");


        public IObservableQuery<T> List { get => _list.Get(); }
        readonly IProperty<IObservableQuery<T>> _list = H<EntityListViewModel<T>>.Property<IObservableQuery<T>>();


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
        readonly ConcurrentDictionary<T, dynamic> _cache = new();


        public new class Injector : EntityListViewModel.Injector
        {
            protected internal IEntityListHelper<T> Helper;
            protected internal Func<T> CreateInstance { get; }
            protected internal IColumnsProvider<T> Columns { get; set; }
            public ILocalizationService Localization { get; }
            public IDocumentService Docs { get; }
            public IDataService Data { get; }
            public IMessagesService Message { get; }

            public Injector(
                EntityListViewModel.Injector innerInjector,
                IEntityListHelper<T> helper,
                Func<T> createInstance,
                IColumnsProvider<T> columnsProvider,

                ILocalizationService localization,
                IDocumentService docs, 
                IMessagesService message, 
                IDataService data

                ) : base(innerInjector)
            {
                Helper = helper;
                CreateInstance = createInstance;
                Columns = columnsProvider;
                Localization = localization;
                Docs = docs;
                Message = message;
                Data = data;
            }
        }

        public override Injector Injected => (Injector)base.Injected;

        protected EntityListViewModel(
            Injector injector,
            Func<IColumnConfigurator<T,object,IFilter<object>>, IDisposable> configurator
           ) : base(injector)
        {

            H<EntityListViewModel<T>>.Initialize(this);

            _list.Set(injector.Columns.List);

            List_CollectionChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, List, 0));
            List.CollectionChanged += List_CollectionChanged;

            configurator.Invoke(new ColumnConfigurator<T,object,IFilter<object>>(this,injector.Localization))?.Dispose();

            Header ??= GetTitle();
            OpenAction ??= target => injector.Docs.OpenDocumentAsync(target);
            IconPath ??= "Icons/Entities/" + typeof(T).Name;
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
            .Action(e => e.List.Refresh())
        );
        
        public override ICommand DeleteCommand { get; } = H<EntityListViewModel<T>>.Command(c => c
            .CanExecute((e, a) =>
            {
                e.RemoveErrorMessage("Delete");
                if(a is T target)
                {
                    return e.CanExecuteDelete(target,m=>e.AddErrorMessage("Delete",m));
                }
                return e.CanExecuteDelete(e.Selected,m=>e.AddErrorMessage("Delete",m));
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
            .Action(async (e,a) => await e.AddEntityAsync(a))
            .On(e => e.Selected)
            .CheckCanExecute()
        );

        public override ICommand ExportCommand { get; } = H<EntityListViewModel<T>>.Command(c => c
            .CanExecute(e =>
            {
                e.RemoveErrorMessage("Export");
                return e.CanExecuteExport(m => e.AddErrorMessage("Export", m));
            })
            .Action(async e => await e.ExportAsync())
            .On(e => e.Selected)
            .CheckCanExecute()
        );

        public override ICommand ImportCommand { get; } = H<EntityListViewModel<T>>.Command(c => c
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

        protected virtual bool CanExecuteDelete(T arg, Action<string> errorAction)
        {
            errorAction("{Delete} : {Not implemented}");
            return false;
        }

        protected virtual bool CanExecuteAdd(Action<string> errorAction)
        {
            errorAction("{Add} : {Not implemented}");
            return false;
        }


        protected virtual bool CanExecuteImport(Action<string> errorAction)
        {
            errorAction("{Import} : {Not implemented}");
            return false;
        }

        protected virtual bool CanExecuteExport(Action<string> errorAction)
        {
            errorAction("{Export} : {Not implemented}");
            return false;
        }
        #endregion



        protected Action<T>? OpenAction;
        public override void SetOpenAction(Action<object> action) => OpenAction = action;
        public void SetOpenAction(Action<T> action) => OpenAction = action;

        protected Action<T> SelectAction;
        public override void SetSelectAction(Action<object> action) => SelectAction = action;
        public void SetSelectAction(Action<T> action) => SelectAction = action;

        protected override Task AddEntityAsync(object arg) => AddEntityAsync();
        protected override Task AddEntityAsync()
        {
            var entity = Injected.CreateInstance();

            //TODO : this is temporary fix data service not injected
            if (entity is IDataServiceProvider {DataService: null} p)
            {
                p.DataService = Injected.Data;
            }

            ConfigureEntity(entity);

            return Injected.Docs.OpenDocumentAsync(entity);
        }

        protected virtual void ConfigureEntity(T entity) { }
        public string Message
        {
            get => _message.Get();
            set => _message.Set(value);
        }

        readonly IProperty<string> _message = H<EntityListViewModel<T>>.Property<string>();


        void List_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newIndex = e.NewStartingIndex;
                    foreach (var n in e.NewItems!.OfType<T>())
                    {
                        ObjectMapper<T> om = _cache.GetOrAdd(n, o => new ObjectMapper<T>(o, Injected.Columns));
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


        readonly IProperty<object> _listCollectionView = H<EntityListViewModel<T>>.Property<object>(c => c
            .Set(setter: e => e.Injected.Helper.GetListView(e.ListViewModel)));

        public override void Populate(object grid) => Injected.Helper.Populate(grid, Injected.Columns);

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
                        Injected.Message.Publish(new DetailMessage(value));
                }
            }
        }

        readonly IProperty<T> _selected = H<EntityListViewModel<T>>.Property<T>();

        public override dynamic SelectedViewModel
        {
            get => _selectedViewModel.Get();
            set => Selected = value?.Model;
        }

        readonly IProperty<dynamic> _selectedViewModel = H<EntityListViewModel<T>>.Property<dynamic>(c => c
            .Set(e => e.Selected == null ? null : e._cache.GetOrAdd(e.Selected, o => new ObjectMapper<T>(o, e.Injected.Columns)))
            .On(e => e.Selected)
            .Update()
        );

        public override IEnumerable<int> SelectedIds
        {
            get => _selectedIds.Get();
            set => _selectedIds.Set(value);
        }

        readonly IProperty<IEnumerable<int>> _selectedIds = H<EntityListViewModel<T>>.Property<IEnumerable<int>>();


        class ExportIdValueProvider : IValueProvider
        {
            readonly IValueProvider _foreignProvider;

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

        Task ExportAsync() => Injected.Helper.ExportAsync(List, new ContractResolver());

        async Task ImportAsync()
        {
            var list = await Injected.Helper.ImportAsync();
            foreach (var entity in list)
            {
                await ImportAsync(Injected.Data, entity);
            }
        }

        protected virtual Task ImportAsync(IDataService data, T importValue) => Task.CompletedTask;

        protected async Task DeleteEntityAsync(T entity)
        {
            await Injected.Docs.CloseDocumentAsync(entity);
            try
            {
                if (await Injected.Data.DeleteAsync(entity))
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
        IColumnsProvider<T> IEntityListViewModel<T>.Columns => Injected.Columns;
    }
}

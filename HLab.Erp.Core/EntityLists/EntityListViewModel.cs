#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using System.Xml.Schema;
using HLab.Base.Extensions;
using HLab.Core.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Core.Tools.Details;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application.Documents;
using HLab.Mvvm.ReactiveUI;
using HLab.Options;
using Newtonsoft.Json.Serialization;
using ReactiveUI;

namespace HLab.Erp.Core.EntityLists;

public abstract class EntityListViewModel : ViewModel
{
   protected class EntityConfigurationException(string message) : Exception(message);

   public bool ShowFilters { get; set => this.RaiseAndSetIfChanged(ref field, value); } = true;

   public bool ShowMenu { get; set => this.RaiseAndSetIfChanged(ref field, value); } = true;

   public bool AllowManualOrder { get; set => this.RaiseAndSetIfChanged(ref field, value); } = false;

   public object Header { get; set => this.RaiseAndSetIfChanged(ref field, value); }

   public string IconPath { get; set => this.RaiseAndSetIfChanged(ref field, value); }

   protected string GetTitle() => $"{{{GetName().FromCamelCase()}}}";
   string GetName() => GetType().Name.BeforeSuffix("ListViewModel");

   public ObservableCollection<IObjectMapper> ListViewModel { get; } = [];

   public void RefreshColumn(string column)
   {
      foreach (var vm in ListViewModel)
      {
         vm.Refresh(column);
      }
   }

   public void RefreshColumn(string column, int id)
   {
      foreach (var vm in ListViewModel.Where(e => e.Id == id))
      {
         vm.Refresh(column);
      }
   }



   public virtual Injector Injected { get; }

   public class Injector(IOptionsService options, Func<Type, object> get)
   {
      public IOptionsService Options { get; } = options;
      internal Func<Type, object> Get = get;

      protected Injector(Injector injector) : this(injector.Options, injector.Get) { }
   }

   protected EntityListViewModel(Injector injector)
   {
      Injected = injector;
      Filters = new ReadOnlyObservableCollection<IFilter>(_filters);

      PopulatePresets();

      FilterPresets = new ReadOnlyObservableCollection<string>(_filterPresets);

      this.WhenAnyValue(e => e.FiltersPresetSelected).Subscribe(c => FilterPresetChanged());

      SaveFiltersPresetCommand = ReactiveCommand.Create(() =>
          {
             SaveFilters();
             PopulatePresets();
          }, this.WhenAnyValue(
              e => e.FiltersPresetName,
              n => !string.IsNullOrEmpty(n))
      );
   }

   void PopulatePresets()
   {
      _filterPresets.Clear();
      var list = Injected.Options.GetOptions($"Filters", GetType().Name, null);
      foreach (var item in list) _filterPresets.Add(item);
   }

   public abstract void Populate(object grid);

   public abstract void SetOpenAction(Action<object> action);
   public abstract void SetSelectAction(Action<object?> action);


   protected readonly ObservableCollection<IFilter> _filters = [];
   public ReadOnlyObservableCollection<IFilter> Filters { get; }


   public void AddFilter(IFilter filter) => _filters.Add(filter);
   public T? GetFilter<T>() where T : IFilter
   {
      var locate = (Func<IEntityListViewModel, T>)Injected.Get(typeof(Func<IEntityListViewModel, T>));
      try
      {
         return locate((IEntityListViewModel)this);
      }
      catch (Exception e)
      {
         Debug.WriteLine($"Error creating filter of type {typeof(T).Name} : {e.Message}");
         return default;
      }
   }

   public abstract void Start();
   public abstract void Stop();



   public abstract dynamic SelectedViewModel { get; set; }

   public IEnumerable<int> SelectedIds { get; set => this.RaiseAndSetIfChanged(ref field, value); }


   public ICommand AddCommand { get; protected set; }
   public ICommand DeleteCommand { get; protected set; }
   public ICommand OpenCommand { get; protected set; }
   public ICommand ImportCommand { get; protected set; }
   public ICommand ExportCommand { get; protected set; }

   public ICommand SaveFiltersPresetCommand { get; }

   void SaveFilters()
   {
      var doc = new XDocument();

      doc.Add(FiltersToXml());

      var xml = doc.ToString();

      Injected.Options.SetValue($"Filters\\{GetType().Name}", FiltersPresetName, xml);
   }


   public string FiltersPresetName { get; set => this.RaiseAndSetIfChanged(ref field, value); }

   public string FiltersPresetSelected { get; set => this.RaiseAndSetIfChanged(ref field, value); }

   void FilterPresetChanged()
   {
      var xml = Injected.Options.GetValue($"Filters\\{GetType().Name}", FiltersPresetSelected, null, () => "");

      if (string.IsNullOrWhiteSpace(xml)) return;

      var doc = XDocument.Parse(xml);

      foreach (var item in doc.Elements())
         FiltersFromXml(item);
   }

   /// <summary>
   /// Type of entity to be passed as argument when adding new entity to list
   /// </summary>
   public virtual Type AddArgumentClass => null;

   public bool IsEnabledSimpleAddButton => AddArgumentClass == null;


   readonly ObservableCollection<string> _filterPresets = new();
   public ReadOnlyObservableCollection<string> FilterPresets { get; }
   public ICommand ShowMenuCommand { get; protected set; }
   public ICommand HideMenuCommand { get; protected set; }


   readonly List<Tuple<string, string>> _errors = new();

   public string ErrorMessage
   {
      get;
      private set => this.RaiseAndSetIfChanged(ref field, value);
   }

   void SetErrorMessage()
   {
      ErrorMessage = string.Join(Environment.NewLine, _errors.OrderBy(e => e.Item1).Select(e => e.Item2));
   }

   public void AddErrorMessage(string key, string message)
   {
      _errors.Add(new(key, message));
      SetErrorMessage();
   }

   public void RemoveErrorMessage(string key)
   {
      _errors.RemoveAll(e => e.Item1 == key);
      SetErrorMessage();
   }

   public XmlSchema GetSchema()
   {
      throw new NotImplementedException();
   }

   public void FiltersFromXml(XElement element)
   {
      var filters = Filters.ToList();

      if (element.Name == "Filters")
      {
         foreach (var child in element.Elements())
         {
            var filter = filters.FirstOrDefault(f => f.Name == child.Name);
            if (filter == null) continue;

            filters.Remove(filter);
            filter.FromXml(child);
            filter.Enabled = true;
         }
      }

      foreach (var filter in filters) filter.Enabled = false;
   }


   public XElement FiltersToXml()
   {
      var xFilters = new XElement("Filters");

      foreach (var filter in Filters)
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
   public IObservableQuery<T> List
   {
      get;
      private init => this.RaiseAndSetIfChanged(ref field, value);
   }


   readonly ConcurrentDictionary<T, dynamic> _cache = new();

   public new class Injector(
       EntityListViewModel.Injector innerInjector,
       IEntityListHelper<T> helper,
       Func<T> createInstance,
       IColumnsProvider<T> columnsProvider,
       ILocalizationService localization,
       IAclService acl,
       IDocumentService docs,
       IMessagesService message,
       IDataService data)
       : EntityListViewModel.Injector(innerInjector)
   {
      protected internal IEntityListHelper<T> Helper = helper;
      protected internal Func<T> CreateInstance { get; } = createInstance;
      protected internal IColumnsProvider<T> ColumnsProvider { get; set; } = columnsProvider;

      public IAclService Acl { get; } = acl;
      public ILocalizationService Localization { get; } = localization;
      public IDocumentService Docs { get; } = docs;
      public IDataService Data { get; } = data;
      public IMessagesService Message { get; } = message;
   }

   public override Injector Injected => (Injector)base.Injected;

   protected EntityListViewModel(
       Injector injector,
       Func<IColumnConfigurator<T>, IDisposable> configurator) : base(injector)
   {

      _listCollectionView = this.WhenAnyValue(
          e => e.ListViewModel,
          selector: e => Injected.Helper.GetListView(e))
      .ToProperty(this, e => e.ListCollectionView);

      List = injector.ColumnsProvider.List;

      List_CollectionChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, List, 0));
      List.CollectionChanged += List_CollectionChanged;

      this.WhenAnyValue(e => e.Selected).Subscribe(e =>
      {
         SelectAction?.Invoke(e);
         Injected.Message.Publish(new DetailMessage(e));
      });

      _selectedViewModel = this.WhenAnyValue(
          e => e.Selected,
          selector: e => e == null ? null : _cache.GetOrAdd(e, o => new ObjectMapper<T>(o, Injected.ColumnsProvider)))
          .ToProperty(this, e => e.SelectedViewModel);

      OpenCommand = ReactiveCommand.Create<object>(
          Open,
          this.WhenAnyValue(
              e => e.Selected,
              selector: OpenCanExecute));

      RefreshCommand = ReactiveCommand.Create(() => List.Refresh());

      DeleteCommand = ReactiveCommand.CreateFromTask<T>(
          DeleteAsync,
          this.WhenAnyValue(
              e => e.Selected,
              selector: DeleteCanExecute));

      AddCommand = ReactiveCommand.CreateFromTask(
          AddAsync,
          this.WhenAnyValue(
              e => e.AddArgumentClass,
              selector: AddCanExecute).SubscribeOn(RxApp.MainThreadScheduler));

      ExportCommand = ReactiveCommand.CreateFromTask(
           ExportAsync,
           this.WhenAnyValue(
           e => e.Selected,
           selector: ExportCanExecute));

      ImportCommand = ReactiveCommand.CreateFromTask(
           ImportAsync,
           this.WhenAnyValue(
           e => e.Selected,
           selector: ImportCanExecute));

      ShowMenuCommand = ReactiveCommand.Create(() => ShowMenu = true);

      HideMenuCommand = ReactiveCommand.Create(() => ShowMenu = false);

      configurator.Invoke(new ColumnConfigurator<T>(this, injector.Localization, injector.Acl))?.Dispose();

      Header ??= GetTitle();
      OpenAction ??= target => injector.Docs.OpenDocumentAsync(target);
      IconPath ??= "Icons/Entities/" + typeof(T).Name;

      injector.ColumnsProvider.SetDefaultOrderBy();
   }



   #region COMMANDS
   void Open(object arg)
   {
      if (arg is T entity)
         OpenAction?.Invoke(entity);
      else if (Selected is { } selected)
         OpenAction?.Invoke(selected);
   }

   bool OpenCanExecute(T? arg)
   {
      RemoveErrorMessage("Open");
      if (arg != null)
         return OpenCanExecute(arg, m => AddErrorMessage("Open", m));

      if (Selected is { } selected)
         return OpenCanExecute(selected, m => AddErrorMessage("Open", m));

      return false;
   }

   public ICommand RefreshCommand { get; }

   bool DeleteCanExecute(T? arg)
   {
      RemoveErrorMessage("Delete");
      if (arg != null)
         return DeleteCanExecute(arg, m => AddErrorMessage("Delete", m));

      if (Selected is { } selected)
         return DeleteCanExecute(selected, m => AddErrorMessage("Delete", m));

      return false;
   }

   async Task DeleteAsync(T? arg)
   {
      if (arg != null)
         await DeleteEntityAsync(arg);
      else if (Selected is { } selected)
         await DeleteEntityAsync(selected);
   }


   Task AddAsync() => AddEntityAsync();

   bool AddCanExecute(Type t)
   {
      RemoveErrorMessage("Add");
      return AddCanExecute(m => AddErrorMessage("Add", m));
   }

   bool ExportCanExecute(T? selected)
   {
      RemoveErrorMessage("Export");
      return ExportCanExecute(m => AddErrorMessage("Export", m));
   }

   bool ImportCanExecute(T? selected)
   {
      RemoveErrorMessage("Import");
      return ImportCanExecute(m => AddErrorMessage("Export", m));
   }


   protected virtual bool OpenCanExecute(T? arg, Action<string> errorAction) => true;

   protected virtual bool DeleteCanExecute(T arg, Action<string> errorAction)
   {
      errorAction("{Delete} : {Not implemented}");
      return false;
   }

   protected virtual bool AddCanExecute(Action<string> errorAction)
   {
      errorAction("{Add} : {Not implemented}");
      return false;
   }

   protected virtual bool ImportCanExecute(Action<string> errorAction)
   {
      errorAction("{Import} : {Not implemented}");
      return false;
   }

   protected virtual bool ExportCanExecute(Action<string> errorAction)
   {
      errorAction("{Export} : {Not implemented}");
      return false;
   }
   #endregion

   // Open Action
   protected Action<T>? OpenAction;
   public override void SetOpenAction(Action<object> action) => OpenAction = action;
   public void SetOpenAction(Action<T> action) => OpenAction = action;

   // Select Action
   protected Action<T?> SelectAction;
   public override void SetSelectAction(Action<object?> action) => SelectAction = action;

   public void SetSelectAction(Action<T?> action) => SelectAction = action;


   public void Drop(object source, object target, bool after)
   {
      if (source is not T fromTest) return;
      if (target is not T toTest) return;
      if (fromTest.Id.Equals(toTest.Id)) return;

      Task.Run(() => ReSortAsync(fromTest, toTest, after));
   }

   protected virtual Task ConfigureNewEntityAsync(T entity, object arg) => ConfigureNewEntityAsync(entity);
   protected virtual Task ConfigureNewEntityAsync(T entity) => Task.CompletedTask;

   async Task AddEntityAsync()
   {
      try
      {
         var entity = await Injected.Data.AddAsync<T>(e => ConfigureNewEntityAsync(e));
         if (entity == null) return;
         await List.UpdateAsync();
         await Injected.Docs.OpenDocumentAsync(entity);
      }
      catch (DataSetterException e)
      {
         Message = e.Message;
      }
      catch (DataException e)
      {
         Message = e.Message;
      }
   }

   public string? Message
   {
      get;
      set => this.RaiseAndSetIfChanged(ref field, value);
   }

   void List_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
   {
      switch (e.Action)
      {
         case NotifyCollectionChangedAction.Add:
            var newIndex = e.NewStartingIndex;
            foreach (var n in e.NewItems!.OfType<T>())
            {
               ObjectMapper<T> om = _cache.GetOrAdd(n, o => new ObjectMapper<T>(o, Injected.ColumnsProvider));
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

   public object ListCollectionView => _listCollectionView.Value;
   readonly ObservableAsPropertyHelper<object> _listCollectionView;


   public override void Populate(object grid) => Injected.Helper.Populate(grid, Injected.ColumnsProvider);

   public T? Selected
   {
      get;
      set => this.RaiseAndSetIfChanged(ref field, value);
   }

   public override dynamic? SelectedViewModel
   {
      get => _selectedViewModel;
      set => Selected = value?.Model;
   }
   readonly ObservableAsPropertyHelper<dynamic?> _selectedViewModel;


   class ExportIdValueProvider(IValueProvider foreignProvider) : IValueProvider
   {
      readonly IValueProvider _foreignProvider = foreignProvider;

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
   IColumnsProvider<T> IEntityListViewModel<T>.Columns => Injected.ColumnsProvider;

   async Task<bool> MoveTo(T entity, int pos)
   {
      if (((IOrderedEntity)entity).Order == pos) return false;

      return await Injected.Data.UpdateAsync(entity, t =>
      {
         ((IOrderedEntity)t).Order = pos;
      });
   }

   async Task ReSortAsync(T move, T to, bool after)
   {
      // Do not move if was dropped on itself
      if (move.Id.Equals(to.Id)) return;

      var update = false;
      var pos = 1;
      // iterate all lines but dropped one
      foreach (var test in List.OrderBy(t => ((IOrderedEntity)t).Order).Where(t => !t.Id.Equals(move.Id)).ToList())
      {
         //did we reach the place to insert
         var insert = test.Id.Equals(to.Id);

         // put dropped if reached location
         if (insert)
         {
            if (after)
            {
               //if dropped after, put current line before
               update |= await MoveTo(test, pos);
               pos++;
               update |= await MoveTo(move, pos);
               pos++;
            }
            else
            {
               //if dropped before, put current line after
               update |= await MoveTo(move, pos);
               pos++;
               update |= await MoveTo(test, pos);
               pos++;
            }
         }
         else
         {
            update |= await MoveTo(test, pos);
            pos++;
         }
      }

      if (update)
      {
         await List.UpdateAsync();
         Selected = move;
      }
   }

}
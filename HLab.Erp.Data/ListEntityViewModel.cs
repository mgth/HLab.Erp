using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using HLab.Base;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Notify;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using NPoco;

////using System.Data.Model;

namespace HLab.Erp.Data
{
    //[Obsolete]
    //public abstract class ListEntityViewModel<T, TLine> : ViewModel, IListEntityViewModel<T, TLine>, INotifierObject
    //    where T : class, INotifyPropertyChanged, IEntity, new()
    //    where TLine : class, INotifyPropertyChanged, INotifierObject, IViewModel<T>, ILineEntityViewModel<T>, new()
    //{
    //    [Import]
    //    private IDbService _db;


    //    private LinkedList<Func<IQueryable<T>,IQueryable<T>>> _searchFuncs = new LinkedList<Func<IQueryable<T>, IQueryable<T>>>();

    //    public T Entity => this.GetModel();

    //    public ListEntityViewModel()
    //    {
    //        //TODO :
    //        //ListCollectionView.IsLiveSorting = true;
    //        //ListCollectionView.IsLiveFiltering = true;
    //        //ListCollectionView.IsLiveGrouping = true;
    //    }

    //    public Type LineViewModelType => typeof(TLine);
    //    public Type EntityViewModelType
    //    {
    //        get => N.Get(() => EntityViewModelTypeDefault); set
    //        {
    //            if (value.IsSubclassOf(EntityViewModelTypeDefault))
    //                N.Set(value);
    //            else
    //            {
    //                throw new ArgumentException(value.FullName + " is not a subclass of " + EntityViewModelTypeDefault.FullName);
    //            }
    //        }
    //    }
    //    private Type EntityViewModelTypeDefault => typeof(IViewModel<>).MakeGenericType(EntityType);
    //    //public virtual UserControl Selector => new DefaultSelectorView();
    //    public virtual IView Selector => null; //TODO : new ListViewDefault();

    //    public string Search
    //    {
    //        get => N.Get(() => ""); set => N.Set(value);
    //    }

    //    public virtual IQueryable<T> Source => N.Get(_db.Get().Set<T>);


    //    [TriggerOn(nameof(Search))]
    //    [TriggerOn(nameof(Source))]
    //    public virtual IQueryable<T> SearchResult => Source;

    //    /// <summary>
    //    /// Open list to select en entity
    //    /// </summary>
    //    /// <param name="item">value to return if cancel</param>
    //    /// <returns>selected entity</returns>
    //    public T GetModel(T item = null)
    //    {
    //        IView view = null;// TODO : new EntitySelectorWindow { DataContext = this };

    //        if (item != null) Select(item);

    //        //if (view.ShowDialog() ?? false)
    //        //{
    //        //    var selectedItem = Selected?.Model;
    //        //    return selectedItem;
    //        //}
    //        return item;
    //    }
    //    public T GetModel(string search, T item = null)
    //    {
    //        Search = search;
    //        return GetModel(item);
    //    }


    //    //public DbContext GetDbContext() => DbService.D.GetDbContext<T>();

    //    //public DbContext DbContext => N.Get(GetDbContext);

    //    //public DbContext DbContextObject => DbContext;

    //    //public DbSet<T> EntityDbSet => N.Get(() => DbContext.Set<T>());

    //    public bool Select(T item)//
    //    {
    //        Selected = ViewModel(item);
    //        return (Selected != null);
    //    }

    //    public bool Select(TLine vm)
    //    {
    //        Selected = vm;
    //        return Selected != null;
    //    }

    //    protected TLine ViewModel(T item)
    //    {
    //        if (item == null) return default(TLine);
    //        using (this.GetNotifier().Suspend.Get())
    //        {
    //            var result = ViewModelCollection.FirstOrDefault(i => i.Model?.Id == item?.Id);
    //            if (result == null)
    //            {
    //                result = new TLine();//Activator.CreateInstance(EntityViewModelType) as TLine;
    //                if (result!=null)
    //                    result.Model = item;
    //            }                
    //            return result;
    //        }
    //    }


    //    public TLine Selected
    //    {
    //        get => N.Get<TLine>(); set => N.Set(value);
    //    }

    //    [TriggerOn(nameof(Selected))]
    //    public object SelectedObjectEntity
    //    {
    //        get => Selected?.Model; set => Select((T)value);
    //    }

    //    [TriggerOn(nameof(Selected))]
    //    public T SelectedEntity
    //    {
    //        get => Selected?.Model; set => Selected = value == null ? null : ViewModel(value);
    //    }

    //    public ObservableCollection<TLine> ViewModelCollection => N.Get(() => new ObservableCollection<TLine>());

    //    //public ListCollectionView ListCollectionView => N.Get(()=> CollectionViewSource.GetDefaultView(ViewModelCollection) as ListCollectionView);

    //    public Type EntityType => typeof(T);

    //    protected virtual Type EditorType => null;

    //    public IView Editor => N.Get(() => EditorType == null ? null : (IView)Activator.CreateInstance(EditorType));

    //    [TriggerOn(nameof(Selected))]
    //    private void UpdateSelected()
    //    {
    //        if (Editor == null) return;
    //        //Editor.DataContext = Selected;
    //    }


    //    private readonly Token _updating = new Token();
    //    [TriggerOn(nameof(SearchResult))]
    //    public void UpdateSource()
    //    {
    //        using (var token = _updating.Get())
    //        {
    //            if (token == null)
    //                return;
    //            HashSet<TLine> oldVms = null;
    //            HashSet<TLine> newVms = null;
    //            //try
    //            {
    //                oldVms = new HashSet<TLine>(ViewModelCollection);
    //                if (SearchResult == null) return;
    //                // Temporary store "from" values cause To creation need to enumerate
    //                // without temp may cause : "There is already an open DataReader associated with this Connection which must be closed first."

    //                HashSet<T> temp;

    //                try
    //                {
    //                    temp = new HashSet<T>(SearchResult);
    //                }
    //                //catch (MySqlException)
    //                //{
    //                //    return;
    //                //}
    //                catch (NullReferenceException)
    //                {
    //                    return;
    //                }

    //                newVms = new HashSet<TLine>();
    //                foreach (var i in temp)
    //                {
    //                    var vm = ViewModel(i);

    //                    if (vm == null) continue;
    //                    using (vm.GetNotifier().Suspend.Get())
    //                    {
    //                        if (vm.ParentObject == null)
    //                        {
    //                            vm.ParentObject = this;
    //                            newVms.Add(vm as TLine);
    //                        }
    //                        else
    //                        {
    //                            oldVms.Remove(vm);
    //                        }
    //                    }
    //                }
    //            }
    //            //catch (Exception ex)
    //            //{

    //            //}
    //            //finally
    //            {
    //                foreach(var vm in oldVms)
    //                    ViewModelCollection.Remove(vm);
    //                foreach (var vm in newVms)
    //                    ViewModelCollection.Add(vm);
                    
    //            }
    //        }
    //    }

    //    private Boxed<ICommand> _addCommand;
    //    public ICommand AddCommand => GetCommand(ref _addCommand, Add,()=>true);
    //    public void Add()
    //    {
    //        Add(Create);
    //    }

    //    public void Add(Func<T, DbContext, bool> create)
    //    {
    //        //using (var ctx = DbService.D.GetDbContext<T>())
    //        {

    //            T entity = (T) Activator.CreateInstance(typeof(T));

    //            using (var db = _db.Get())
    //            {
    //                db.Set<T>().Add(entity);
    //                if (create(entity, db))
    //                {
    //                    Select(entity);
    //                }

    //                db.SaveChanges();
    //            }
    //        }
    //    }


    //    public ICommand RemoveCommand => this.GetCommand(Remove,()=>true);
    //    private void Remove()
    //    {
    //        var old = Selected;
    //        while (Selected != null)
    //        {
    //            Delete(Selected);
    //            if (old != Selected)
    //                old = Selected;
    //            else Selected = null;
    //        }
    //    }

    //    public virtual bool Create(T entity, DbContext ctx)
    //    {
    //        ctx.Set<T>().Add(entity);
    //        ctx.SaveChanges();
    //        UpdateSource();
    //        return true;
    //    }


    //    public ListAdder<T> Adder() => new ListAdder<T>(this,_db); 

    //    public virtual bool Delete(TLine vm)
    //    {

    //        var item = (vm.Model);
    //        //var ctx = item?.GetNewDbContext();

    //        //DbSet<T> dbset = ctx?.Set<T>();

    //        if (item == null) return false;

    //        using (item.GetNotifier().Suspend.Get())
    //        {
    //            _db.Get().Remove(item);
    //            _db.Get().SaveChanges();
    //        }
    //        return ViewModelCollection.Remove(vm);
    //    }

    //    public INotifier GetNotifier()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class ListAdder<T> : IDisposable
        where T : class, INotifierObject//, new()
    {
        public T Entity { get; }
        private readonly IListEntityViewModel<T> _list;
        private readonly IDatabase _db;

        private readonly SuspenderToken _suspend;

        //Todo : Adder there
        public ListAdder(IListEntityViewModel<T> list, IDataService db)
        {
            _list = list;

            Entity = (T)Activator.CreateInstance(typeof(T));
                

            _db.Insert(Entity);//_set.Create();
            //_db.SaveChanges();

            _suspend = Entity.GetNotifier().Suspend.Get();
        }

        public void Dispose()
        {
            _db.Delete<T>(Entity);
            //_db.SaveChanges();
            _list.UpdateSource();
            _list.Select(Entity);
            _db.Dispose();
            _suspend.Dispose();
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;


////using System.Data.Model;

namespace HLab.Erp.Core.Wpf.Lists.QueryLists;

public interface IQueryList
{
    Type ViewMode { get; set; }
    Type EntityType { get; }

    //ModelCommand CreateCommand { get; }
    //ModelCommand DeleteCommand { get; }
}

public abstract class QueryListBase<T> : ObservableCollection<T>, IQueryList, IViewModel
{
    //public ModelCommand CreateCommand => this.GetCommand(Create, ()=>true);
    //public ModelCommand DeleteCommand => this.GetCommand(Delete, ()=>false);

    protected abstract void Create();
    protected abstract void Delete();

    public Type ViewMode
    {
        get => _viewMode;
        set => this.SetAndRaise(ref _viewMode,value);
    }
    Type _viewMode = typeof(DefaultViewMode);

    public abstract Type EntityType { get; }
    public IMvvmContext MvvmContext { get; set; }
    public Type ModelType => null;

    public object Model { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}




public class QueryListBase<T, TVm, TThis> : QueryListBase<TVm>
    where TThis : QueryListBase<T, TVm, TThis>
    where TVm : IViewModel<T>//, INotifyPropertyChanged
    where T : class, IEntity, new()


{
        
    public IMvvmService Mvvm { get; set; }
    public IDataService Db { get; set; }
    public class CreateHelper : IDisposable
    {
        public TThis List = null;
        public T Entity = null;
        public IDataService Db { get; }
        public bool Done = true;

        public CreateHelper(IDataService _db)
        {
            Db = _db;
        }

        public void Dispose()
        {

        }
    }

    class Filter
    {
        public object Name { get; set; }
        public Func<IQueryable<T>, IQueryable<T>> Func { get; set; }
        public int Order { get; set; }
    }

    class PostFilter
    {
        public object Name { get; set; }
        public Func<IEnumerable<T>, IEnumerable<T>> Func { get; set; }
        public int Order { get; set; }
    }



    public override Type EntityType => typeof(T);

    readonly object _lockFilters = new object();
    readonly List<Filter> _filters = new List<Filter>();
    readonly List<PostFilter> _postFilters = new List<PostFilter>();


    public QueryListBase(INotifyPropertyChanged parentViewModel, Func<IQueryable<T>> sourceFunc, Type viewMode) 
    {
        SourceFunc = sourceFunc;
        //ParentViewModel = parentViewModel;
        ViewMode = viewMode;
    }

    public T SelectedEntity
    {
        get => _selectedEntity;
        set => Selected = this.FirstOrDefault(e => e.Model.Id == value.Id);
    }

    T _selectedEntity = H<QueryListBase<T, TVm, TThis>>.Property<T>(c => c
        .Set(e => e.Selected.Model)
        .On(e => e.Selected.Model).Update()
    );

    public Func<IQueryable<T>> SourceFunc
    {
        get => _sourceFunc;
        set => this.SetAndRaise(ref _sourceFunc,value);
    }

    Func<IQueryable<T>> _sourceFunc = H<QueryListBase<T, TVm, TThis>>.Property<Func<IQueryable<T>>>();

    //public IQueryProvider<T> Source => N.Get(() =>
    //{
    //    //_ctx?.Dispose();
    //    if (_ctx == null) _ctx = DbService.D.Db;
    //    return SourceQuery(_ctx);
    //});

    public IQueryable<T> Source => SourceFunc();

    public IQueryable<T> Query
    {
        get
        {
            var s = Source;

            if (s == null) return null;

            lock (_lockFilters)
                foreach (var filter in _filters.OrderBy(f => f.Order))
                {
                    s = filter.Func(s);
                }
            return s;
        }
    }

    public IEnumerable<T> PostQuery
    {
        get
        {
            IEnumerable<T> q = Query.ToList();
            if (q == null) return null;

            lock (_lockFilters)
                foreach (var filter in _postFilters.OrderBy(f => f.Order))
                {
                    q = filter.Func(q);
                }
            return q;

        }
    }

    public TThis AddFilter(object name, Expression<Func<T, bool>> expression, int order = 0)
    {
        return AddFilter(expression, order, name);
    }

    public TThis AddFilter(Expression<Func<T, bool>> expression, int order = 0, object name = null)
    {
        return AddFilter(s => s.Where(expression), order, name);
    }

    public TThis AddFilter(Func<IQueryable<T>, IQueryable<T>> func, int order = 0, object name = null)
    {
        lock (_lockFilters)
        {
            if (name != null) RemoveFilter(name);
            _filters.Add(new Filter
            {
                Name = name,
                Func = func,
                Order = order,
            });
            return (TThis)this;
        }
    }
    public TThis AddPostFilter(string name, Func<T, bool> expression, int order = 0)
    {
        return AddPostFilter(expression, order, name);
    }

    public TThis AddPostFilter(Func<T, bool> expression, int order = 0, string name = null)
    {
        return AddPostFilter(s => s.Where(expression), order, name);
    }

    public TThis AddPostFilter(Func<IEnumerable<T>, IEnumerable<T>> func, int order = 0, string name = null)
    {
        lock (_lockFilters)
        {
            if (name != null) RemoveFilter(name);
            _postFilters.Add(new PostFilter
            {
                Name = name,
                Func = func,
                Order = order,
            });
            return (TThis)this;
        }
    }

    public TThis RemoveFilter(object name)
    {
        lock (_lockFilters)
        {
            foreach (var f in _filters.Where(f => Equals(f.Name,name)).ToList())
            {
                _filters.Remove(f);
            }
            return (TThis)this;
        }
    }
    public TThis RemovePostFilter(object name)
    {
        lock (_lockFilters)
        {
            foreach (var f in _postFilters.Where(f => Equals(f.Name,name)).ToList())
            {
                _postFilters.Remove(f);
            }
            return (TThis)this;
        }
    }


    //public Func<TThis, T, DbContext, bool> OnCreate;
    protected readonly Dictionary<string, Action<CreateHelper>> OnCreate = new Dictionary<string, Action<CreateHelper>>();
    protected readonly Dictionary<string, Action<CreateHelper>> OnCreated = new Dictionary<string, Action<CreateHelper>>();

    protected readonly Dictionary<string, Action<CreateHelper>> OnDelete = new Dictionary<string, Action<CreateHelper>>();
    protected readonly Dictionary<string, Action<CreateHelper>> OnDeleted = new Dictionary<string, Action<CreateHelper>>();
    protected override void Create()
    {
        var entity = new T();

        using (var helper = new CreateHelper(Db) { List = (TThis)this, Entity = entity })
            if (Exec(OnCreate, helper))
            {
                //TODO : 
                //.Add(()=>entity);
                //helper.Context.SaveChanges();

                SelectedEntity = entity;
                Update();
                Exec(OnCreated, helper);
            }
    }

    [TriggerOn(nameof(Selected))]
    public void UpdateRemoveCommand()
    {
        var canexecute = Selected != null;
        //DeleteCommand.SetCanExecute(canexecute);
    }


    protected override void Delete()
    {
        var entity = SelectedEntity;

        if (entity != null)
        {
            using (var helper = new CreateHelper(Db) { List = (TThis)this, Entity = entity })
                if (Exec(OnDelete, helper))
                {
                    // TODO: 
                    //helper.Context.Remove(Selected);
                    //helper.Context.SaveChanges();

                    Update();

                    Exec(OnDeleted, helper);
                }
        }
    }

    TThis AddCreator(Dictionary<string, Action<CreateHelper>> dict, Action<CreateHelper> func,
        string name = "")
    {
        lock (_lockFilters)
        {
            if (name != null && dict.ContainsKey(name)) dict.Remove(name);
            dict.Add(name ?? "", func);
            return (TThis)this;
        }
    }

    protected bool Exec(Dictionary<string, Action<CreateHelper>> dict, CreateHelper h)
    {
        foreach (var action in dict.Values)
        {
            action(h);
            // if one action fail, we don't want to create new entity.
            if (!h.Done) return false;
        }
        return true;
    }

    public TThis AddOnCreate(Action<CreateHelper> func, string name = "")
    {
        return AddCreator(OnCreate, func, name);
    }
    public TThis AddOnCreated(Action<CreateHelper> func, string name = "")
    {
        return AddCreator(OnCreate, func, name);
    }
    public TThis AddOnDelete(Action<CreateHelper> func, string name = "")
    {
        return AddCreator(OnCreate, func, name);
    }
    public TThis AddOnDeleted(Action<CreateHelper> func, string name = "")
    {
        return AddCreator(OnCreate, func, name);
    }

    //public ModelCommand GetEntityInteractifCommand => this.GetCommand(GetEntityInteractif,()=>true);

    public void GetEntityInteractif()
    {
        //Selected = GetEntityInteractif(Selected);
    }

    //public T GetEntityInteractif(T item)
    //{
    //    var view = new EntitySelectorWindow { DataContext = this };

    //    if (item != null) Select(item);

    //    if (view.ShowDialog() ?? false)
    //    {
    //        var selectedItem = Selected?.Model;
    //        //GetDbContext.Entry(selectedItem).State = EntityState.Detached;
    //        return selectedItem;
    //    }
    //    return item;
    //}


    readonly object _lockUpdate = new object();

    public TThis Update(bool forceUpdate = false)
    {
        //Todo : multithreading
#if MULTITHREAD
                var t = new Thread(() =>

    {
                try
                {
                    Monitor.Enter(_lockUpdateNeeded);
                    _updateNeeded = true;
                    lock (_lockUpdate)
                    {
                        Monitor.Exit(_lockUpdateNeeded);
                        UpdateThread();
                    }
                }
                finally { if(Monitor.IsEntered(_lockUpdateNeeded)) Monitor.Exit(_lockUpdateNeeded); }
            }
            );
            t.Start();
#else
        UpdateThread(forceUpdate);
#endif
        return (TThis)this;
    }

    readonly object _lockUpdateNeeded = new object();
    volatile bool _updateNeeded = false;


    volatile bool _updating = false;

    public void UpdateThread(bool forceUpdate = false)
    {
        lock (_lockUpdate)
        {
            if (_updating) return;
            _updating = true;
            _updateNeeded = false;
        }


        var changed = false;


        var n = 0;
        //using (new DebugTimer("=== FluentUpdate ==="))
        // TODO : using (N.Suspend.Get())
        {
            //DbContext.LogToConsole();

            // NullException : often occur when data is not nullable but sql is
            var list = PostQuery?.ToList()/**/;

            if (list == null) return;

            foreach (var item in list)
            {
                if (_updateNeeded) return;
                // while list is consistant
                if (n < Count && item.Equals(this[n].Model))
                {
                    if (forceUpdate)
                    {
                        var vm1 = this[n];
                        Remove(vm1);
                        Insert(n, vm1);
                    }
                    n++;
                    continue;
                }

                //next item exists elswhere in collection
                var vm = this.FirstOrDefault(e => e.Model.Equals(item));
                if (vm != null)
                {
                    //if item found before current position it's duplicate items 
                    if (IndexOf(vm) <= n) continue;

                    Remove(vm);
                    Insert(n, vm);
                    n++;
                    continue;
                }

                //var entity = EntityHelper<T>.Load(item);
                vm = (TVm)Mvvm.MainContext.GetLinked(item, ViewMode,typeof(IDefaultViewClass));

                if (vm != null)
                {
                    //vm.ParentViewModel = ParentViewModel;
                    //InitAction?.Invoke(vm);
                    Insert(n, vm);
                    n++;
                    changed = true;
                }
                //next item needs to be created
            }

            //remove remaining items
            while (n < Count) RemoveAt(n);
        }

        if (!changed)
        {
        }

        _updating = false;
    }

}
﻿//#define MULTITHREAD

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using HLab.Base;
using HLab.Core.DebugTools;
using HLab.DependencyInjection.Annotations;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

////using System.Data.Entity;

namespace HLab.Erp.Data.Observables
{
    //public interface IObservableQuery
    //{
    //    IEntity SelectedEntity { get; set; }
    //    void Update();
    //}
    //public interface IObservableQuery<T> : ITriggerable, IObservableQuery, IList<T>
    //    where T : class, IEntity
    //{
    //    IObservableQuery<T> AddFilter(Expression<Func<T, bool>> expression, int order = 0, string name = null);
    //    IObservableQuery<T> SetSource(Func<IEnumerable<T>> src);
    //    IObservableQuery<T> SetSource(Func<IQueryable<T>, IQueryable<T>> src);
    //    void Update(bool force = true);
    //}

    public static class ObservableQueryExtensions
    {
        public static ObservableQuery<T> AddFilter<T>(this ObservableQuery<T> oq, string name, Func<Expression<Func<T, bool>>> expression, int order = 0)
        where T : class, IEntity
        {
            oq.AddFilter(expression, order, name);
            return oq;
        }

        public static ObservableQuery<T> FluentUpdate<T>(this ObservableQuery<T> oq, bool force = true)
            where T : class, IEntity
        {
            oq.UpdateAsync(force);
            return oq;
        }
    }

    [Export(typeof(ObservableQuery<>))]
    public class ObservableQuery<T> : ObservableCollectionNotifier<T>, ITriggerable//, IObservableQuery<T>
        where T : class, IEntity
    {
        [Import]
        public ObservableQuery(IDataService db)
        {
            _db = db;
            H.Initialize(this,OnPropertyChanged);
            Suspender = new Suspender(()=>UpdateAsync());
        }

        protected new class H : NotifyHelper<ObservableQuery<T>> { }

        private readonly IDataService _db;
        public Suspender Suspender { get; }

        //public ModelCommand CreateCommand => this.GetCommand(Create, ()=>true);
        //public ModelCommand DeleteCommand => this.GetCommand(Delete, ()=>false);

        private IAsyncEnumerable<T> _source = null;

        public class CreateHelper : IDisposable
        {
            public ObservableQuery<T> List = null;
            public T Entity = null;
            public bool Done = true;

            public void Dispose()
            {
//                Context?.Dispose();
            }
        }
        private class Filter
        {
            public string Name { get; set; }
            public Func<Expression<Func<T, bool>>> GetExpression { get; set; } = null;
            public Func<IQueryable<T>, IQueryable<T>> Func { get; set; } = null;
            public int Order { get; set; }
        }
        private class PostFilter
        {
            public string Name { get; set; }
            public Func<T, bool> Expression { get; set; } = null;
            public Func<IAsyncEnumerable<T>, IAsyncEnumerable<T>> Func { get; set; }
            public int Order { get; set; }
        }

        private readonly ReaderWriterLockSlim _lockFilters = new ReaderWriterLockSlim();
        private readonly List<Filter> _filters = new List<Filter>();
        private readonly List<PostFilter> _postFilters = new List<PostFilter>();

        public Expression<Func<T, object>> OrderBy
        {
            get => _orderBy;
            set => _orderBy = value;
        }

        //public new IEntity Selected
        //{
        //    get => base.Selected;
        //    set => base.Selected = (T)value;
        //}
        //private IProperty<IEntity> _selectedEntity = H.Property<IEntity>( c => c
        //    .On(e => e.Selected)
        //    .Set(e => e.Selected)
        //    );
        public ObservableQuery<T> SetSource(Func<IAsyncEnumerable<T>> src)
        {
            SourceEnumerable = src;
            return this;
        }

        public ObservableQuery<T> SetSource(Func<IQueryable<T>, IQueryable<T>> src)
        {
            SourceQuery = src;
            return this;
        }

        public Func<IQueryable<T>, IQueryable<T>> SourceQuery
        {
            get => _sourceQuery.Get();
            set => _sourceQuery.Set(value);
        }
        private readonly IProperty<Func<IQueryable<T>, IQueryable<T>>> _sourceQuery = H.Property<Func<IQueryable<T>, IQueryable<T>>>(c => c
           .Set(e =>(Func<IQueryable<T>, IQueryable<T>>)(q => q))
        );

        public Func<IAsyncEnumerable<T>> SourceEnumerable
        {
            get => _sourceEnumerable.Get();
            set => _sourceEnumerable.Set(value);
        }
        private readonly IProperty<Func<IAsyncEnumerable<T>>> _sourceEnumerable = H.Property<Func<IAsyncEnumerable<T>>>();


        //private IQueryable<T> Query(IQueryable<T> s)
        //{
        //    if (s == null) return null;
            
        //    lock (_lockFilters)
        //        foreach (Filter filter in _filters.OrderBy(f => f.Order))
        //        {

        //            s = filter.Func != null ? filter.Func(s) : s.Where(filter.GetExpression());
        //        }
        //    return s;
        //}

        private Expression<Func<T,bool>> Where()
        {
            Expression<Func<T,bool>> result = null;
            //ParameterExpression param = null;

            _lockFilters.EnterReadLock();
            try
            {
                foreach (var filter in _filters.OrderBy(f => f.Order))
                {
                    result = result.AndAlso(filter.GetExpression());
                }
            }
            catch
            {
                return t => true;
            }
            finally
            {
                _lockFilters.ExitReadLock();
            }

            if (result == null)
                return t => true;

            return result;
        }

        //TODO : use where function to compile expression and cache it
        private IAsyncEnumerable<T> PostQuery(IAsyncEnumerable<T> q)
        {
            if (q == null) return null;
            _lockFilters.EnterReadLock();
            try
            {
                foreach (var filter in _postFilters.OrderBy(f => f.Order))
                {
                    q = filter.Func != null ? filter.Func(q) : q.Where(filter.Expression);
                }
            }
            finally
            {
                _lockFilters.ExitReadLock();
            }

            return q;
        }
        private IAsyncEnumerable<T> PostQueryAsync(bool force = true)
        {
            if (_source == null || force)
            {
                if (SourceEnumerable != null)
                {
                    _source = SourceEnumerable?.Invoke();
                }
                else
                {
                    _source = _db.FetchWhereAsync(Where(),_orderBy);
                }                
            }
            return PostQuery(_source);
        }


        public ObservableQuery<T> AddFilter(Func<Expression<Func<T, bool>>> expression, int order = 0, string name = null)
        {
            _lockFilters.EnterWriteLock();
            try
            {
                if (name != null) RemoveFilter(name);
                _filters.Add(new Filter
                {
                    Name = name,
                    GetExpression = expression,
                    Order = order,
                });
                return this;
            }
            finally
            {
                if(_lockFilters.IsWriteLockHeld) 
                    _lockFilters.ExitWriteLock();
            }
        }

        public ObservableQuery<T> AddFilterFunc(Func<IQueryable<T>, IQueryable<T>> func, int order = 1, string name = null)
        { 
            _lockFilters.EnterWriteLock(); 
            try
            {
                if (name != null) RemoveFilter(name);
                _filters.Add(new Filter
                {
                    Name = name,
                    Func = func,
                    Order = order,
                });
                return this;
            }
            finally
            {
                if(_lockFilters.IsWriteLockHeld) 
                    _lockFilters.ExitWriteLock();
            }
        }
        public ObservableQuery<T> AddPostFilter(string name, Func<T, bool> expression, int order = 0)
        {
            return AddPostFilter(expression, order, name);
        }

        public ObservableQuery<T> AddPostFilter(Func<T, bool> expression, int order = 0, string name = null)
        {
            _lockFilters.EnterWriteLock(); 
            try
            {
                if (name != null) RemoveFilter(name);
                _postFilters.Add(new PostFilter
                {
                    Name = name,
                    Expression = expression,
                    Order = order,
                });
                return this;
            }
            finally
            {
                if(_lockFilters.IsWriteLockHeld) 
                    _lockFilters.ExitWriteLock();
            }
            //return AddPostFilter(s => s.Where(expression), order, name);
        }

        public ObservableQuery<T> AddPostFilter(Func<IAsyncEnumerable<T>, IAsyncEnumerable<T>> func, int order = 0, string name = null)
        {
            _lockFilters.EnterWriteLock(); 
            try
            {
                if (name != null) RemovePostFilter(name);
                _postFilters.Add(new PostFilter
                {
                    Name = name,
                    Func = func,
                    Order = order,
                });
                return this;
            }
            finally
            {
                if(_lockFilters.IsWriteLockHeld) 
                    _lockFilters.ExitWriteLock();
            }
        }
        public ObservableQuery<T> RemoveFilter(string name)
        {
            bool locked = _lockFilters.IsWriteLockHeld;
            if(!locked) _lockFilters.EnterWriteLock(); 
            try
            {
                foreach (Filter f in _filters.Where(f => f.Name == name).ToList())
                {
                    _filters.Remove(f);
                }
                return this;
            }
            finally
            {
                if(!locked && _lockFilters.IsWriteLockHeld) 
                    _lockFilters.ExitWriteLock();
            }
        }

        public ObservableQuery<T> RemovePostFilter(string name)
        {
            _lockFilters.EnterWriteLock(); 
            try
            {
                foreach (var f in _postFilters.Where(f => f.Name == name).ToList())
                {
                    _postFilters.Remove(f);
                }
                return this;
            }
            finally
            {
                if(_lockFilters.IsWriteLockHeld) 
                    _lockFilters.ExitWriteLock();
            }
        }

        //public Func<TThis, T, DbContext, bool> OnCreate;
        protected readonly ConcurrentDictionary<string, Action<CreateHelper>> OnCreate = new ConcurrentDictionary<string, Action<CreateHelper>>();
        protected readonly ConcurrentDictionary<string, Action<CreateHelper>> OnCreated = new ConcurrentDictionary<string, Action<CreateHelper>>();

        protected readonly ConcurrentDictionary<string, Action<CreateHelper>> OnDelete = new ConcurrentDictionary<string, Action<CreateHelper>>();
        protected readonly ConcurrentDictionary<string, Action<CreateHelper>> OnDeleted = new ConcurrentDictionary<string, Action<CreateHelper>>();
        protected void Create()
        {
            //TODO
            //var entity = new T();

            //using (var helper = new CreateHelper { List = this, Entity = entity })
            //    if (Exec(OnCreate, helper))
            //    {
            //        //TODO
            //        //helper.Context.Insert(entity);

            //        Selected = entity;
            //        FluentUpdate();
            //        Exec(OnCreated, helper);
            //    }
        }
        protected void Delete()
        {
            var entity = Selected;
            if (entity == null) return;

            if (Exec(OnDelete, new CreateHelper
            {
                List = this,
                Entity = entity,
            }))
            {
                _db.Delete(Selected);
                UpdateAsync();

                Exec(OnDeleted, new CreateHelper
                {
                    List = this,
                    Entity = entity,
                });
            }           
        }

        private ObservableQuery<T> AddCreator(IDictionary<string, Action<CreateHelper>> dict, Action<CreateHelper> func,
            string name = "")
        {
            _lockFilters.EnterWriteLock(); 
            try
            {
                if (name != null && dict.ContainsKey(name)) dict.Remove(name);
                dict.Add(name ?? "", func);
                return this;
            }
            finally
            {
                if(_lockFilters.IsWriteLockHeld) 
                    _lockFilters.ExitWriteLock();
            }
        }

        protected bool Exec(ConcurrentDictionary<string, Action<CreateHelper>> dict, CreateHelper h)
        {
            foreach (var action in dict.Values)
            {
                action(h);
                // if one action fail, we don't want to create new entity.
                if (!h.Done) return false;
            }
            return true;
        }

        public ObservableQuery<T> AddOnCreate(Action<CreateHelper> func, string name = "")
        {
            return AddCreator(OnCreate, func, name);
        }
        public ObservableQuery<T> AddOnCreated(Action<CreateHelper> func, string name = "")
        {
            return AddCreator(OnCreated, func, name);
        }
        public ObservableQuery<T> AddOnDelete(Action<CreateHelper> func, string name = "")
        {
            return AddCreator(OnDelete, func, name);
        }
        public ObservableQuery<T> AddOnDeleted(Action<CreateHelper> func, string name = "")
        {
            return AddCreator(OnDeleted, func, name);
        }

//        public ModelCommand GetEntityInteractifCommand => this.GetCommand(GetEntityInteractif,()=>true);

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
        //        var selectedItem = Selected?.Entity;
        //        //GetDbContext.Entry(selectedItem).State = EntityState.Detached;
        //        return selectedItem;
        //    }
        //    return item;
        //}





        private readonly object _lockUpdate = new object();

        public ObservableQuery<T> Do(Action<ObservableQuery<T>> action)
        {
            action(this);
            return this;
        }

        private Thread _updateThread = null;

        public ObservableQuery<T> FluentUpdateAsync(bool force = true)
        {
            var oldThread = _updateThread;

            _updateThread = new Thread(()=>
            {
                lock (_lockUpdate)
                {
                    if(oldThread?.IsAlive??false)  _updateNeeded = true;
                }

                oldThread?.Join();

                UpdateAsync(force);
            });
            _updateThread.Start();
            return this;
        }

        private readonly object _lockUpdateNeeded = new object();
        private volatile bool _updateNeeded = false;


        private volatile bool _updating = false;

        private bool _initialized = false;
        private Expression<Func<T, object>> _orderBy;

        public ObservableQuery<T> Init()
        {
            if (!_initialized)
            {
                UpdateAsync();
            }
            return this;
        }

        public Task  UpdateAsync() => UpdateAsync(true,false);
        public Task  UpdateAsync(bool force) => UpdateAsync(force,false);
        public Task  RefreshAsync() => UpdateAsync(true,true);

        public async Task UpdateAsync(bool force,bool refresh)
        {
            if (Suspender.Suspended) return;

            var stopwatch = Stopwatch.StartNew();
            lock (_lockUpdate)
            {
                _updateNeeded = false;
            }

            var removed = new Dictionary<object,T>();

            Lock.EnterWriteLock();
            try
            {
                {
                    var list = PostQueryAsync(force) /*.ToList()*/;

                    Console.WriteLine("Query : " + stopwatch.ElapsedMilliseconds);

                    if (list == null) return;

                    if(refresh)
                    {
                        while(Count>0)
                            RemoveAtNoLock(0);
                    }

                    var n = 0;
                    await foreach (var item in list)
                    {
                        //var item = list[n];
                        var id = item.Id;

                        if (_updateNeeded)
                        {
                            lock (_lockUpdate)
                            {
                                _updating = false;
                                _updateNeeded = false;
                            }

                            return;
                        }

                        // while list is consistent
                        if (n < Count)
                        {
                            if (Equals(id, this[n].Id))
                            {
                                n++;
                                continue;
                            }

                            if (this.Any(e => Equals(e.Id, item.Id)))
                            {
                                var old = this[n];
                                while (!Equals(old.Id,item.Id))
                                {
                                    RemoveAtNoLock(n);
                                    old = this[n];
                                }

                                n++;
                                continue;
                            }
                        } 
                        
                        InsertNoLock(n, item);
                        n++;
                    }

                    Console.WriteLine("Update : " + stopwatch.ElapsedMilliseconds);

                    //remove remaining items
                    while (n < Count)
                    {
                        RemoveAtNoLock(n);
                    }

                    Console.WriteLine("Cleanup : " + stopwatch.ElapsedMilliseconds);
                    Debug.Assert(Count == n);

                }
                
                _initialized = true;
                _updating = false;
            }
            finally
            {
                Lock.ExitWriteLock();
                OnCollectionChanged();
            }
        }

        //take care of modified entity that do not match filters anymore
        private void Vm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var vm = sender as T;
            if (!Match(vm)) Remove(vm);
        }

        private bool Match(T vm)
        {
            if (vm != null)
            {
                return _filters.Where(filter => filter.GetExpression != null).All(filter => filter.GetExpression().Compile()(vm)) 
                    && _postFilters.Where(filter => filter.Expression != null).All(filter => filter.Expression(vm));
            }
            return false;
        }

        public void OnTriggered()
        {
            UpdateAsync();
        }

        public override void Add(T item)
        {
            throw new NotImplementedException("Observable Query is readOnly");
        }
        public override void Insert(int index, T item)
        {
            throw new NotImplementedException("Observable Query is readOnly");
        }
    }
}

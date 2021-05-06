//#define MULTITHREAD

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Grace.DependencyInjection.Attributes;
using HLab.Base;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

////using System.Data.Entity;

namespace HLab.Erp.Data.Observables
{
    public interface IObservableQuery
    {
        void Update();
    }
    public interface IObservableQuery<T> : ITriggerable, IObservableQuery, IList<T>
    {
        Suspender Suspender { get; }

        void AddFilter(Expression<Func<T, bool>> filter, int order = 0, object name = null);
        void AddFilter(Func<Expression<Func<T, bool>>> filter, int order = 0, object name = null);
        void RemoveFilter(object header);
        void AddPostFilter(object header, Func<T, bool> postMatch);
    }

    [Export(typeof(ObservableQuery<>))]
    public class ObservableQuery<T> : ObservableCollectionNotifier<T>, IObservableQuery<T>, ITriggerable//, IObservableQuery<T>
        where T : class, IEntity
    {

        private readonly IGuiTimer _timer;

        public ObservableQuery(IDataService db)
        {
            _db = db;
            H<ObservableQuery<T>>.Initialize(this);

            _timer = NotifyHelper.EventHandlerService.GetTimer();
            _timer.Tick += async (a, b) => await _timer_TickAsync(a, b);
            _timer.Interval = TimeSpan.FromMilliseconds(300);

            Suspender = new Suspender(() => _timer.Stop(), () => _timer.Start());

            _token = Suspender.Get();
        }

        private readonly IDataService _db;

        private SuspenderToken _token;
        public Suspender Suspender { get; }

        public void Start()
        {
            if (_token != null)
            {
                var token = _token;
                _token = null;
                token.Dispose();
            }
        }


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
            public object Name { get; set; }
            public Func<Expression<Func<T, bool>>> GetExpression { get; init; } = null;
            public Func<IQueryable<T>, IQueryable<T>> Func { get; init; } = null;
            public int Order { get; init; }
        }
        private class PostFilter
        {
            public object Name { get; init; }
            public Func<T, bool> Expression { get; init; } = null;
            public Func<IAsyncEnumerable<T>, IAsyncEnumerable<T>> Func { get; init; }
            public int Order { get; init; }
        }

        private class OrderByEntry
        {
            public object Name { get; init; }
            public Func<T, object> Expression { get; set; }
            public bool Descending { get; set; }
            public int Order { get; init; }
        }

        private readonly object _lockFilters = new object();
        private List<Filter> _filters = new();
        private List<PostFilter> _postFilters = new();
        private List<OrderByEntry> _orderBy = new();

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
        private readonly IProperty<Func<IQueryable<T>, IQueryable<T>>> _sourceQuery = H<ObservableQuery<T>>.Property<Func<IQueryable<T>, IQueryable<T>>>(c => c
           .Set(e => (Func<IQueryable<T>, IQueryable<T>>)(q => q))
        );

        public Func<IAsyncEnumerable<T>> SourceEnumerable
        {
            get => _sourceEnumerable.Get();
            set => _sourceEnumerable.Set(value);
        }
        private readonly IProperty<Func<IAsyncEnumerable<T>>> _sourceEnumerable = H<ObservableQuery<T>>.Property<Func<IAsyncEnumerable<T>>>();

        private Expression<Func<T, bool>> Where()
        {

            var filters = _filters;
#if !DEBUG
            try
            {
#endif
            Expression<Func<T, bool>> result = null;
            foreach (var filter in filters.OrderBy(f => f.Order))
            {
                result = result.AndAlso(filter.GetExpression());
            }
#if DEBUG
            var literal = result?.ToString() ?? "NULL";
#endif
            return result ?? (t => true);

#if !DEBUG
            }
            catch
            {
                return t => true;
            }
#endif
        }

        //TODO : use where function to compile expression and cache it
        private IAsyncEnumerable<T> PostQuery(IAsyncEnumerable<T> q)
        {
            if (q == null) return null;
            var filters = _postFilters;
            foreach (var filter in filters.OrderBy(f => f.Order))
            {
                q = filter.Func != null ? filter.Func(q) : q.Where(filter.Expression);
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
                    _source = _db.FetchWhereAsync(Where());
                }
            }
            return PostQuery(_source);
        }

        public ObservableQuery<T> AddFilter(Expression<Func<T, bool>> expression, int order = 0, object name = null)
            => AddFilter(() => expression, order, name);
        void IObservableQuery<T>.AddFilter(Func<Expression<Func<T, bool>>> filter, int order, object name)
            => AddFilter(filter, order, name);

        void IObservableQuery<T>.RemoveFilter(object header) => RemoveFilter(header);

        void IObservableQuery<T>.AddPostFilter(object header, Func<T, bool> postMatch)
            => AddPostFilter(header, postMatch);

        public ObservableQuery<T> AddFilter(Func<Expression<Func<T, bool>>> expression, int order = 0, object name = null)
        {
            lock (_lockFilters)
            {
                var filters = _filters.ToList();

                if (name != null)
                {
                    var filter = filters.FirstOrDefault(f => Equals(f.Name, name));
                    if (filter != null) filters.Remove(filter);
                }
                filters.Add(new Filter
                {
                    Name = name,
                    GetExpression = expression,
                    Order = order,
                });

                Interlocked.Exchange(ref _filters, filters);

                return this;
            }
        }

        public ObservableQuery<T> AddFilterFunc(Func<IQueryable<T>, IQueryable<T>> func, int order = 1, object name = null)
        {
            lock (_lockFilters)
            {
                var filters = _filters.ToList();

                if (name != null)
                {
                    var filter = filters.FirstOrDefault(f => Equals(f.Name, name));
                    if (filter != null) filters.Remove(filter);
                }
                filters.Add(new Filter
                {
                    Name = name,
                    Func = func,
                    Order = order,
                });
                Interlocked.Exchange(ref _filters, filters);
                return this;
            }
        }
        public ObservableQuery<T> AddPostFilter(object name, Func<T, bool> expression, int order = 0)
        {
            return AddPostFilter(expression, order, name);
        }

        public ObservableQuery<T> AddPostFilter(Func<T, bool> expression, int order = 0, object name = null)
        {
            lock (_lockFilters)
            {
                var filters = _postFilters.ToList();

                if (name != null)
                {
                    var filter = filters.FirstOrDefault(f => Equals(f.Name, name));
                    if (filter != null) filters.Remove(filter);
                }
                filters.Add(new PostFilter
                {
                    Name = name,
                    Expression = expression,
                    Order = order,
                });
                Interlocked.Exchange(ref _postFilters, filters);
                return this;
            }
        }

        public ObservableQuery<T> ResetOrderBy()
        {
            lock (_lockFilters)
            {
                Interlocked.Exchange(ref _orderBy, new List<OrderByEntry>());
                return this;
            }
        }

        public ObservableQuery<T> AddOrderBy(Func<T, object> expression, bool descending = false, int order = 0, string name = null)
        {
            lock (_lockFilters)
            {
                var orderByList = _orderBy.ToList();

                if (name != null)
                {
                    var filter = orderByList.FirstOrDefault(f => Equals(f.Name, name));
                    if (filter != null) orderByList.Remove(filter);
                }
                orderByList.Add(new OrderByEntry
                {
                    Name = name,
                    Expression = expression,
                    Descending = descending,
                    Order = order,
                });
                Interlocked.Exchange(ref _orderBy, orderByList);
                return this;
            }
        }

        public ObservableQuery<T> AddPostFilter(Func<IAsyncEnumerable<T>, IAsyncEnumerable<T>> func, int order = 0, string name = null)
        {
            lock (_lockFilters)
            {
                var filters = _postFilters.ToList();

                if (name != null)
                {
                    var filter = filters.FirstOrDefault(f => Equals(f.Name, name));
                    if (filter != null) filters.Remove(filter);
                }
                filters.Add(new PostFilter
                {
                    Name = name,
                    Func = func,
                    Order = order,
                });
                Interlocked.Exchange(ref _postFilters, filters);
                return this;
            }
        }
        public ObservableQuery<T> RemoveFilter(object name)
        {
            lock (_lockFilters)
            {
                var filters = _filters.ToList();

                foreach (var f in filters.Where(f => Equals(f.Name, name)).ToList())
                {
                    filters.Remove(f);
                }
                Interlocked.Exchange(ref _filters, filters);
                return this;
            }
        }

        public ObservableQuery<T> RemovePostFilter(string name)
        {
            lock (_lockFilters)
            {
                var filters = _postFilters.ToList();
                foreach (var f in filters.Where(f => Equals(f.Name, name)).ToList())
                {
                    filters.Remove(f);
                }
                Interlocked.Exchange(ref _postFilters, filters);
                return this;
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
                Update();

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
            lock (_lockFilters) // Todo : insuffisent 
            {
                if (name != null && dict.ContainsKey(name)) dict.Remove(name);
                dict.Add(name ?? "", func);
                return this;
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

            _updateThread = new Thread(() =>
            {
                lock (_lockUpdate)
                {
                    if (oldThread?.IsAlive ?? false) _updateNeeded = true;
                }

                oldThread?.Join();

                UpdateAsync(force);
            });
            _updateThread.Start();
            return this;
        }

        private readonly object _lockUpdateNeeded = new();


        private volatile bool _updateNeeded;


        private volatile bool _updating = false;


        public void Update()
        {
            lock (_lockUpdateNeeded)
            {
                _updateNeeded = true;
            }
        }

        public Task UpdateAsync(Action postUpdate) => UpdateAsync(postUpdate, true, false);
        public Task UpdateAsync(bool force) => UpdateAsync(null, force, false);
        public Task RefreshAsync() => UpdateAsync(null, true, true);


        private async Task _timer_TickAsync(object sender, EventArgs e)
        {
            var doUpdate = false;
            lock (_lockUpdateNeeded)
            {
                if (_updateNeeded)
                {
                    _updateNeeded = false;
                    doUpdate = true;
                    _timer.Start();
                }
            }

            if (doUpdate)
            {
                await UpdateAsync(null, true, false);
            }
        }


        public async Task UpdateAsync(Action postUpdate, bool force, bool refresh)
        {
#if DEBUG
            var stopwatch = Stopwatch.StartNew();
#endif
            var lck = await Lock.WriterLockAsync();
            try
            {
                using (lck)
                {
#if ASYNCQUERY
                    var list = PostQueryAsync(force);
#else
                    var list = PostQueryAsync(force);
#endif
                    foreach (var orderBy in _orderBy.OrderBy(o => o.Order))
                    {
                        if (list is IOrderedAsyncEnumerable<T> ordered)
                            list = orderBy.Descending ? ordered.ThenByDescending(orderBy.Expression) : ordered.ThenBy(orderBy.Expression);
                        else
                            list = orderBy.Descending ? list.OrderByDescending(orderBy.Expression) : list.OrderBy(orderBy.Expression);
                    }

#if DEBUG
                    var queryWatch = stopwatch.ElapsedMilliseconds;
#endif

                    if (list == null) return;

                    if (refresh)
                    {
                        while (Count > 0)
                            RemoveAtNoLock(0);
                    }

                    var n = 0;
                    await
                        foreach (var item in list.ConfigureAwait(true))
                    {
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
                            var old = GetNoLock(n);

                            if (Equals(id, old.Id))
                            {
                                n++;
                                continue;
                            }

                            if (this.Any(e => Equals(e.Id, item.Id)))
                            {
                                while (!Equals(old.Id, item.Id))
                                {
                                    RemoveAtNoLock(n);
                                    old = GetNoLock(n);
                                }

                                n++;
                                continue;
                            }
                        }

                        InsertNoLock(n, item);
                        n++;
                    }

#if (DEBUG)
                    var updateWatch = stopwatch.ElapsedMilliseconds;
#endif

                    //remove remaining items
                    while (n < Count)
                    {
                        RemoveAtNoLock(n);
                    }

#if DEBUG
                    var cleanupWatch = stopwatch.ElapsedMilliseconds;

                    Debug.WriteLine($"Query {typeof(T).Name} : {queryWatch}");
                    Debug.WriteLine($"Update {typeof(T).Name} : {updateWatch - queryWatch}");
                    Debug.WriteLine($"Cleanup {typeof(T).Name} : {cleanupWatch - updateWatch}");
                    Debug.Assert(Count == n);
#endif
                }

                _updating = false;

                postUpdate?.Invoke();
            }
            finally
            {
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

        void IObservableQuery<T>.AddFilter(Expression<Func<T, bool>> filter, int order, object name) => AddFilter(filter,order,name);

        public void OnTriggered()
        {
            Update();
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

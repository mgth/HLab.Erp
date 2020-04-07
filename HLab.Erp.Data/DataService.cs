using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using HLab.DependencyInjection.Annotations;
using HLab.Base;
using HLab.Core.Annotations;
using Npgsql;
using NPoco;

namespace HLab.Erp.Data
{

    public class DataException : Exception
    {
        public DataException(String message, Exception inner) : base(message, inner)
        {

        }
    }

    public class DataTransaction : IDataTransaction
    {
        private DataService _service;
        private ITransaction _transaction;
        internal IDatabase Database;
        private Action _rollback = default(Action);

        public DataTransaction(DataService service, IDatabase database)
        {
            _service = service;
            Database = database;
            _transaction = database.GetTransaction();
        }

        public T Add<T>(Action<T> setter, Action<T> added = null) where T : class, IEntity
        {
            var t = (T)Activator.CreateInstance(typeof(T));  //_entityFactory(typeof(T));
            //if(t is IEntity<int> tt) tt.Id=-1;

            setter?.Invoke(t);

            object e = null;
            if (typeof(T).GetCustomAttributes<SoftIncrementAttribut>().FirstOrDefault() is SoftIncrementAttribut a)
            {
                if (t is IEntity<int> ti)
                {
                        var ids = Database.Query<T>().OrderByDescending(d => ((IEntity<int>)d).Id).FirstOrDefault();

                        var id = ((IEntity<int>)ids)?.Id ?? 0;

                        id++;

                        ti.Id = id;
                }
            }

            e = Database.Insert(t);

            if (e != null)
            {
                t.IsLoaded = true;
                added?.Invoke(t);
            }

            return _service.GetCache<T>().GetOrAddAsync(t).Result;
        }

        public async Task<T> AddAsync<T>(Action<T> setter, Action<T> added = null) where T : class, IEntity
        {
            var t = (T)Activator.CreateInstance(typeof(T));  //_entityFactory(typeof(T));
            if(t is IEntity<int> tt) tt.Id=-1;

            setter?.Invoke(t);

            object e = null;
            if (typeof(T).GetCustomAttributes<SoftIncrementAttribut>().FirstOrDefault() is SoftIncrementAttribut a)
            {
                if (t is IEntity<int> ti)
                {
                        var ids = await Database.QueryAsync<T>().OrderByDescending(d => ((IEntity<int>) d).Id)
                            .FirstOrDefault().ConfigureAwait(false);

                        var id = ((IEntity<int>) ids)?.Id ?? 0;

                        id++;

                        ti.Id = id;

                }
            }

            e = await Database.InsertAsync(t).ConfigureAwait(false);

            if (e != null)
            {
                t.IsLoaded = true;
                added?.Invoke(t);
            }

            return await _service.GetCache<T>().GetOrAddAsync(t).ConfigureAwait(false);
        }

        public void Update<T>(T value, IEnumerable<string> columns) where T : class, IEntity
        {
            Database.Update(value, columns);
        }

        public async Task<bool> UpdateAsync<T>(T value, IEnumerable<string> columns) where T : class, IEntity
        {
            var n = await Database.UpdateAsync(value, columns);
            return n > 0;
        }

        public void Save<T>(T value) where T : class, IEntity
        {
            Database.Save(value);
        }

        public Task SaveAsync<T>(T value) where T : class, IEntity
        {
            return Task.Run(() => Database.Save(value));
        }
        public int Delete<T>(T entity, Action<T> deleted = null)
            where T : class, IEntity
        {
            var result = Database.Delete<T>(entity);

            if(result>0)
                _service.GetCache<T>().ForgetAsync(entity);

            if (result > 0) deleted?.Invoke((T)entity);

            return result;
        }
        public async Task<int> DeleteAsync<T>(T entity, Action<T> deleted = null)
            where T : class, IEntity
        {
            var result = await Database.DeleteAsync(entity);

            if (result > 0) 
                await _service.GetCache<T>().ForgetAsync(entity);

            if (result > 0) 
                deleted?.Invoke((T)entity);
            
            return result;
        }

        public void Done()
        {
            _transaction.Complete();
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }
    }



    [Export(typeof(IDataService)), Singleton]
    public class DataService : IDataService
    {
        public async Task<T> AddAsync<T>(Action<T> setter, Action<T> added = null)
            where T : class, IEntity
        {
            var t = (T)Activator.CreateInstance(typeof(T));  //_entityFactory(typeof(T));
            if(t is IEntity<int> tt) tt.Id=-1;

            setter?.Invoke(t);

            object e = null;
            if (typeof(T).GetCustomAttributes<SoftIncrementAttribut>().FirstOrDefault() is SoftIncrementAttribut a)
            {
                if (t is IEntity<int> ti)
                {
                    await DbAsync(async db =>
                    {
                        var ids = await db.QueryAsync<T>().OrderByDescending(d => ((IEntity<int>) d).Id)
                            .FirstOrDefault().ConfigureAwait(false);

                        var id = ((IEntity<int>) ids)?.Id ?? 0;

                        id++;

                        ti.Id = id;

                    });
                }
            }

            e = await DbGetAsync(async db => await db.InsertAsync(t).ConfigureAwait(false));

            if (e != null)
            {
                t.IsLoaded = true;
                added?.Invoke(t);
            }

            return await GetCache<T>().GetOrAddAsync(t).ConfigureAwait(false);
        }
        public T Add<T>(Action<T> setter, Action<T> added = null)
            where T : class, IEntity
        {
            var t = (T)Activator.CreateInstance(typeof(T));  //_entityFactory(typeof(T));
            //if(t is IEntity<int> tt) tt.Id=-1;

            setter?.Invoke(t);

            object e = null;
            if (typeof(T).GetCustomAttributes<SoftIncrementAttribut>().FirstOrDefault() is SoftIncrementAttribut a)
            {
                if (t is IEntity<int> ti)
                {
                    Db(db =>
                    {
                        var ids = db.Query<T>().OrderByDescending(d => ((IEntity<int>)d).Id).FirstOrDefault();

                        var id = ((IEntity<int>)ids)?.Id ?? 0;

                        id++;

                        ti.Id = id;
                    });
                }
            }

            e = DbGet(db => db.Insert(t));

            if (e != null)
            {
                t.IsLoaded = true;
                added?.Invoke(t);
            }

            return GetCache<T>().GetOrAddAsync(t).Result;
        }





        public bool Delete<T>(T entity, Action<T> deleted = null)
            where T : class, IEntity
        {
            var result = DbGet(db => db.Delete<T>(entity));

            if (result > 0) {
                var b = GetCache<T>().ForgetAsync(entity).Result;
                deleted?.Invoke((T)entity);
                return true;
            }

            return false;
        }
        public async Task<bool> DeleteAsync<T>(T entity, Action<T> deleted = null)
            where T : class, IEntity
        {
            var result = await DbGetAsync(async db => await db.DeleteAsync(entity));

            if (result > 0)
            {
                await GetCache<T>().ForgetAsync(entity);
                deleted?.Invoke((T)entity);
                return true;
            }
            
            return false;
        }

        public async Task<T> GetOrAddAsync<T>(Expression<Func<T, bool>> getter, Action<T> setter, Action<T> added = null)
            where T : class, IEntity
        {
            using(var transaction = GetTransaction() as DataTransaction)
            {
                var t = await transaction.Database.QueryAsync<T>().FirstOrDefault(getter);

                if (t == null)
                {

                    var result = transaction.Add(setter, added);
                    transaction.Done();
                    return result;
                }

                return await GetCache<T>().GetOrAddAsync(t).ConfigureAwait(false);
            }

        }
        public T GetOrAdd<T>(Expression<Func<T, bool>> getter, Action<T> setter, Action<T> added = null)
            where T : class, IEntity
        {
            using(var transaction = GetTransaction() as DataTransaction)
            {
                var t = transaction.Database.Query<T>().FirstOrDefault(getter);

                if (t == null)
                {

                    var result = transaction.Add(setter, added);
                    transaction.Done();
                    return result;
                }

                return GetCache<T>().GetOrAdd(t);
            }

        }
        public Task<T> GetOrAddAsync<T>(T entity)
            where T : class, IEntity
        {
            return GetCache<T>().GetOrAddAsync(entity);
        }
        //public static IQueryProviderWithIncludes<T> Query<T>() => D.Get().Query<T>();
        public async IAsyncEnumerable<T> FetchAsync<T>() where T : class, IEntity
        {
            var cache = GetCache<T>();

            using var db = Get();

            await using var enumerator = db.FetchAsync<T>().GetAsyncEnumerator();

            var more = true;

            while(more)
            {
                try
                {
                    more = await enumerator.MoveNextAsync();
                }
                catch (NpgsqlException e)
                {
                    throw new DataException(e.Message,e);
                }

                if (more) yield return await cache.GetOrAddAsync(enumerator.Current).ConfigureAwait(false);
            }
        }

        public void Execute(Action<IDatabase> action)
        {
            Db(action);
        }

//        public List<T> FetchQuery<T>(Func<IQueryable<T>, IQueryable<T>> q)
        public IAsyncEnumerable<T> FetchWhereAsync<T>(Expression<Func<T, bool>> expression, Expression<Func<T, object>> orderBy = null)
            where T : class, IEntity
        {
            var cache = GetCache<T>();


            if (typeof(ILocalCache).IsAssignableFrom(typeof(T)))
            {
                var list = cache.FetchAsync(expression);
                if (orderBy != null)
                {
                    var o = orderBy.Compile();
                    list = list.OrderBy(o);
                }

                return list;
            }

            using var db = Get();
            {
                var list =
                    db.QueryAsync<T>().Where(expression);

                if (orderBy != null) list = list.OrderBy(orderBy);

                return cache.GetOrAddAsync(list.ToEnumerable());
            }
        }

        public async Task<T> FetchOneAsync<T>(Expression<Func<T, bool>> expression)
            where T : class, IEntity
        {
            var result = await DbGetAsync(async db => await db.QueryAsync<T>().Where(expression).FirstOrDefault().ConfigureAwait(false)).ConfigureAwait(false);

            return result == null ? null : await GetCache<T>().GetOrAddAsync(result).ConfigureAwait(false);
        }


        public T FetchOne<T>(Expression<Func<T, bool>> expression)
            where T : class, IEntity
        {
            var result = DbGet(db => db.Query<T>().Where(expression).FirstOrDefault());
            return result == null ? null : GetCache<T>().GetOrAddAsync(result).Result;
        }

        public Task<T> FetchOneAsync<T>(int id)
            where T : class, IEntity<int>

            => FetchOneAsync<T>((object)id);

        public Task<T> FetchOneAsync<T>(string id)
            where T : class, IEntity<string>

            => FetchOneAsync<T>((object)id);

        public async Task<T> ReFetchOneAsync<T>(T entity)
            where T : class, IEntity
        {
            if(entity==null) return null;

            var result = await DbGetAsync(async db => await db.SingleByIdAsync<T>(entity.Id).ConfigureAwait(false)).ConfigureAwait(true);
            return await GetCache<T>().GetOrAddAsync(result).ConfigureAwait(true);
        }

        public async Task<T> FetchOneAsync<T>(object id)
            where T : class, IEntity
        {
            var subscribe = false;

            var obj = await GetCache<T>().GetOrAddAsync(id,
                 async k =>
                {
                    subscribe = true;
                    return await DbGetAsync(async db => await db.SingleOrDefaultByIdAsync<T>(k).ConfigureAwait(false)).ConfigureAwait(false);
                }).ConfigureAwait(false);

            //if(subscribe && obj is INotifierObject nobj) nobj.GetNotifier().Subscribe();
            if (subscribe && obj is IEntity entity) entity.OnLoaded();
            if (subscribe && obj is IDataProvider dbf) dbf.DataService = this;

            return obj;

        }

        public bool Any<T>(Expression<Func<T, bool>> expression)
            where T : class, IEntity
        {
                return DbGet<bool>(db => db.Query<T>().Any(expression));
        }

        private readonly ConcurrentDictionary<Type, DataCache> _caches = new ConcurrentDictionary<Type, DataCache>();

        internal DataCache<T> GetCache<T>() where T : class, IEntity
            => (DataCache<T>)_caches.GetOrAdd(typeof(T), t => new DataCache<T>() { DataService = this });


        public string ConnectionString { get; private set; }
        private string _password;
        private string _driver;
        private int _nbOpen = 0;

        [Import]
        public Func<Type, object> _Locate { get; }
        public object Locate<T>(DbDataReader d) => _Locate(typeof(T));

        private IExportLocatorScope _container;

        [Import(InjectLocation.AfterConstructor)]
        public void Inject(IExportLocatorScope container, IOptionsService opt)
        {
            opt.SetDataService(this);
            RegisterEntities(container);
            var connectionString = opt.GetOptionString("Connection");
            Register(connectionString,"");
        }


        public void RegisterEntities(IExportLocatorScope container)
        {
            _container = container;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypesSafe().Where(t => t.IsClass && !t.IsAbstract && typeof(IEntity).IsAssignableFrom(t)))
                {
                    Entities.Add(type);
                }
            }
        }

        public List<Type> Entities { get; } = new List<Type>();

        //[Import] private Func<IDataService, IDatabase> _getDbContext;
        internal IDatabase Get()
        {
            var db = new Database(
                ConnectionString,
                DatabaseType.PostgreSQL, 
                //DatabaseType.MySQL,
                //MySql.Data.MySqlClient.MySqlClientFactory.Instance,
                Npgsql.NpgsqlFactory.Instance
            );

            return db;
        }

        public IDataTransaction GetTransaction()
        {
            return new DataTransaction(this,Get());
        }

        public void Register(string connectionString, string driver)
        {
            ConnectionString = connectionString;
            _driver = driver;
        }

        public DbTransaction BeginTransaction() => Get().Transaction;

        public void Save<T>(T value) where T : class, IEntity
        {
            Db(d => d.Save(value));
        }
        public async Task SaveAsync<T>(T value) where T : class, IEntity
        {
            await DbAsync(async d => await Task.Run(() => d.Save(value)));
        }

        public bool Update<T>(T value, IEnumerable<string> columns) where T : class, IEntity
        {
            var n = DbGet(d => d.Update(value, columns));
            return (n > 0);
        }
        public async Task<bool> UpdateAsync<T>(T value, IEnumerable<string> columns) where T : class, IEntity
        {
            var n = await DbGetAsync(d => d.UpdateAsync(value,columns));
            return (n > 0);
        }

        public IAsyncEnumerable<TSelect> SelectDistinctAsync<T, TSelect>(Func<T, bool> expression, Func<T, TSelect> select)
        {
            using var d = Get();
            return d.FetchAsync<T>().Where(expression).Select(select).Distinct();
        }

        private bool Retry()
        {
            return true;
        }


        private void Db(Action<IDatabase> action) => DbGet(db =>
        {
            action(db);
            return true;
        });
        private T DbGet<T>(Func<IDatabase,T> action)
        {
            while (true)
            {
                try
                {
                    using var d = Get();
                    return action(d);
                }
                catch (NpgsqlException exception)
                {
                    throw new DataException("Data connection failed",exception);
                    Thread.Sleep(5000); 
                }
            }
        }

        //private async Task DbTryAsync(Func<IDatabase,Task> action)
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            using var d = Get();
        //            await action(d);
        //            return;
        //        }
        //        catch (NpgsqlException exception)
        //        {
        //            Thread.Sleep(5000); 
        //        }
        //    }

        //}
        private async Task DbAsync(Func<IDatabase,Task> action) => await DbGetAsync(async db =>
        {
            await action(db);
            return true;
        });
        private async Task<T> DbGetAsync<T>(Func<IDatabase,Task<T>> action)
        {
            while (true)
            {
                try
                {
                    using var d = Get();
                    return await action(d);
                }
                catch (NpgsqlException exception)
                {
                    throw new DataException("Data connection failed",exception);
                }
            }

        }
    }
}

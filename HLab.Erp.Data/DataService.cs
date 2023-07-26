using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using HLab.Base;
using HLab.Core.Annotations;
using Npgsql;
using NPoco;
using HLab.Options;

namespace HLab.Erp.Data
{
    public class DataSetterException : Exception
    {
        public DataSetterException(string message) : base(message) { }
    }

    public class DataService : IDataService, IService
    {
        readonly IOptionsService _options;
        public Func<Type, object> Locate { get; private set;}

        public DataService(Func<Type, object> locate, IOptionsService options)
        {
            Locate = locate;
            _options = options;
            DataCache.DataService = this;
        }


        public async Task<T> AddAsync<T>(Action<T> setter, Action<T> added = null)
            where T : class, IEntity
        {
            var t = (T)Activator.CreateInstance(typeof(T));  //_entityFactory(typeof(T));
            if (t is IEntity<int> tt) tt.Id = -1;

            try
            {
                setter?.Invoke(t);
            }
            catch (DataSetterException)
            {
                return null;
            }

            object e = null;
            if (typeof(T).GetCustomAttributes<SoftIncrementAttribut>().FirstOrDefault() is { } a)
            {
                if (t is IEntity<int> ti)
                {
                    await DbAsync(async db =>
                    {
                        var ids = await db.QueryAsync<T>().OrderByDescending(d => ((IEntity<int>)d).Id)
                            .FirstOrDefault().ConfigureAwait(false);

                        var id = ((IEntity<int>)ids)?.Id ?? 0;

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

            return await DataCache<T>.Cache.GetOrAddAsync(t).ConfigureAwait(false);
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

            return DataCache<T>.Cache.GetOrAddAsync(t).Result;
        }





        public bool Delete<T>(T entity, Action<T> deleted = null)
            where T : class, IEntity
        {
            var result = DbGet(db => db.Delete<T>(entity));

            if (result > 0)
            {
                var b = DataCache<T>.Cache.ForgetAsync(entity).Result;
                deleted?.Invoke((T)entity);
                return true;
            }

            return false;
        }
        public async Task<bool> DeleteAsync<T>(T entity, Action<T> deleted = null)
            where T : class, IEntity
        {
            if (entity == null) return false;

            var result = await DbGetAsync(async db => await db.DeleteAsync(entity));

            if (result > 0)
            {
                await DataCache<T>.Cache.ForgetAsync(entity);
                deleted?.Invoke((T)entity);
                return true;
            }

            return false;
        }

        public async Task<T> GetOrAddAsync<T>(Expression<Func<T, bool>> getter, Action<T> setter, Action<T> added = null)
            where T : class, IEntity
        {
            using (var transaction = GetTransaction() as DataTransaction)
            {
                var t = await transaction.Database.QueryAsync<T>().FirstOrDefault(getter);

                if (t == null)
                {

                    var result = transaction.Add(setter, added);
                    transaction.Done();
                    return result;
                }

                return await DataCache<T>.Cache.GetOrAddAsync(t).ConfigureAwait(false);
            }

        }
        public T GetOrAdd<T>(Expression<Func<T, bool>> getter, Action<T> setter, Action<T> added = null)
            where T : class, IEntity
        {
            try
            {

                using var transaction = GetTransaction() as DataTransaction;

                if (transaction != null)
                {
                    var t = transaction.Database.Query<T>().FirstOrDefault(getter);

                    if (t == null)
                    {

                        var result = transaction.Add(setter, added);
                        transaction.Done();
                        return result;
                    }

                    return DataCache<T>.Cache.GetOrAdd(t);
                }
                throw new DataException("Unable to create transaction");
            }
            catch (Exception ex)
            {
                throw new DataException("Unable to create transaction",ex);
            }

        }
        public Task<T> GetOrAddAsync<T>(T entity)
            where T : class, IEntity
        {
            return DataCache<T>.Cache.GetOrAddAsync(entity);
        }
        //public static IQueryProviderWithIncludes<T> Query<T>() => D.Get().Query<T>();
        public async IAsyncEnumerable<T> FetchAsync<T>() where T : class, IEntity
        {
            var cache = DataCache<T>.Cache;

            var retry = true;
            while (retry)
            {
                retry = false;
                using var db = Get();
                await using var enumerator = db.QueryAsync<T>().ToEnumerable().GetAsyncEnumerator();
                {
                    var more = true;

                    while (more)
                    {
                        try
                        {
                            more = await enumerator.MoveNextAsync();
                        }
                        catch (ArgumentException e)
                        {
                            more = false;
                            retry = Configure(e);
                        }
                        catch (NpgsqlException e)
                        {
                            throw new DataException(e.Message, e);
                        }

                        if (more) yield return await cache.GetOrAddAsync(enumerator.Current).ConfigureAwait(false);
                    }

                }
            }
        }

        public void Execute(Action<IDatabase> action)
        {
            Db(action);
        }

        public async Task ExecuteSqlAsync(string sql)
        {
            using var db = Get();
            await db.ExecuteAsync(sql);
        }

        public int ExecuteSql(string sql)
        {
            try
            {
                using var db = Get();
                return db.Execute(sql);
            }
            catch (Exception)
            {
                return -1;
            }
        }
        public IEnumerable<T> FetchWhere<T>(Expression<Func<T, bool>> expression, Expression<Func<T, object>> orderBy = null)
            where T : class, IEntity
        {
            var cache = DataCache<T>.Cache;


            if (typeof(ILocalCache).IsAssignableFrom(typeof(T)))
            {
                var list = cache.Fetch(expression);
                if (orderBy != null)
                {
                    var o = orderBy.Compile();
                    list = list.OrderBy(o);
                }

                return list;
            }

            using var db = Get();
            {
                #if DEBUG
                var literal = expression.ToString();
                #endif
                var list =
                    db.Query<T>().Where(expression);

                if (orderBy != null) list = list.OrderBy(orderBy);

                var retry = true;
                Exception ex = null;
                while (retry)
                {
                    retry = false;
                    try
                    {
                        return cache.GetOrAdd(list.ToEnumerable());

                    }
                    catch (ArgumentException e)
                    {
                        ex = e;
                        retry = Configure(e);
                    }
                }

                throw new DataException(ex.Message, ex);
            }
        }

        public IAsyncEnumerable<T> FetchWhereAsync<T>(Expression<Func<T, bool>> expression, Expression<Func<T, object>> orderBy = null)
            where T : class, IEntity
        {
            var cache = DataCache<T>.Cache;


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
                #if DEBUG
                var literal = expression.ToString();
                #endif
                var list =
                    db.QueryAsync<T>().Where(expression);

                if (orderBy != null) list = list.OrderBy(orderBy);

                var retry = true;
                Exception ex = null;
                while (retry)
                {
                    retry = false;
                    try
                    {
                        return cache.GetOrAddAsync(list.ToEnumerable());

                    }
                    catch (ArgumentException e)
                    {
                        ex = e;
                        retry = Configure(e);
                    }
                }

                throw new DataException(ex.Message, ex);
            }
        }

        public async Task<T> FetchOneAsync<T>(Expression<Func<T, bool>> expression)
            where T : class, IEntity
        {
            var result = await DbGetAsync(async db => await db.QueryAsync<T>().Where(expression).FirstOrDefault().ConfigureAwait(false)).ConfigureAwait(false);

            return result == null ? null : await DataCache<T>.Cache.GetOrAddAsync(result).ConfigureAwait(false);
        }



        public T FetchOne<T>(Expression<Func<T, bool>> expression)
            where T : class, IEntity
        {
            var result = DbGet(db => db.Query<T>().Where(expression).FirstOrDefault());
            return result == null ? null : DataCache<T>.Cache.GetOrAddAsync(result).Result;
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
            if (entity == null) return null;

            var result = await DbGetAsync(async db => await db.SingleByIdAsync<T>(entity.Id).ConfigureAwait(false)).ConfigureAwait(true);
            return await DataCache<T>.Cache.GetOrAddAsync(result).ConfigureAwait(true);
        }

        public async Task<T> FetchOneAsync<T>(object id)
            where T : class, IEntity
        {
            var subscribe = false;

            var obj = await DataCache<T>.Cache.GetOrAddAsync(id,
                 async k =>
                {
                    subscribe = true;
                    return await DbGetAsync(async db => await db.SingleOrDefaultByIdAsync<T>(k).ConfigureAwait(false)).ConfigureAwait(false);
                }).ConfigureAwait(false);

            //if(subscribe && obj is INotifierObject nobj) nobj.GetNotifier().Subscribe();
            if (subscribe && obj is IEntity entity) entity.OnLoaded();
            if (subscribe && obj is IDataServiceProvider dbf) dbf.DataService = this;

            return obj;

        }

        public bool Any<T>(Expression<Func<T, bool>> expression)
            where T : class, IEntity
        {
            return DbGet<bool>(db => db.Query<T>().Any(expression));
        }

        IEnumerable<string> _connectionStrings;
        public IEnumerable<string> Connections
        {
            get
            {
                if (_connectionStrings is null)
                {
                    _connectionStrings = _options.GetSubList("", "Connections", null, "registry");
                }
                return _connectionStrings;
            }
        }

        string _connectionString;
        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_connectionString))
                {
                    var path = (string.IsNullOrWhiteSpace(Source)) ? "" : @$"Connections\{Source}";
                    _connectionString = _options.GetValue<string>(path, "Connection", null, null, "registry");
                    if (string.IsNullOrWhiteSpace(_connectionString))
                    {
                        _connectionString = _getConnectionString();
                        _options.SetValue(path, "connection", _connectionString);
                    }
                }
                return _connectionString;
            }
        }

        string _source;

        public string Source
        {
            get => _source ??= _options.GetValue("", "Source", null, ()=>"", "registry");
            set
            {
                _source = value;
                _options.SetValue<string>("", "Source", value, "registry");
                _connectionString = null;
//                _caches.Clear();
            }
        }

        //public object Locate<T>(DbDataReader d) => _Locate(typeof(T));



        public void RegisterEntities(Action<Type> action)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypesSafe().Where(t => t.IsClass && !t.IsAbstract && typeof(IEntity).IsAssignableFrom(t)))
                {
                    action(type);
                    Entities.Add(type);
                }
            }
        }

        public List<Type> Entities { get; } = new List<Type>();

        internal IDatabase Get()
        {
            var db = new Database(
                ConnectionString,
                DatabaseType.PostgreSQL,
                NpgsqlFactory.Instance
            );

            return db;
        }

        public IDataTransaction GetTransaction()
        {
            return new DataTransaction(this, Get());
        }


//        public DbTransaction BeginTransaction() => Get().Transaction;

        public void Save<T>(T value) where T : class, IEntity
        {
            Db(d => d.Save(value));
        }
        public async Task SaveAsync<T>(T value) where T : class, IEntity
        {
            await DbAsync(async d => await Task.Run(() => d.Save(value)));
        }

        public bool Update<T>(T value, params string[] columns) where T : class, IEntity
        {
            var n = DbGet(d => d.Update(value, columns));
            return (n > 0);
        }
        public async Task<bool> UpdateAsync<T>(T value, params string[] columns) where T : class, IEntity
        {
            var n = await DbGetAsync(d => d.UpdateAsync(value, columns));
            return (n > 0);
        }

        public bool Update<T>(T value, Action<T> setter) where T : class, IEntity
        {
            var persister = new EntityPersister<T>(this, value);
            setter(value);
            return persister.Save();
        }
        public Task<bool> UpdateAsync<T>(T value, Action<T> setter) where T : class, IEntity
        {
            var persister = new EntityPersister<T>(this, value);
            setter(value);
            return persister.SaveAsync();
        }

        public IAsyncEnumerable<TSelect> SelectDistinctAsync<T, TSelect>(Expression<Func<T, bool>> expression, Func<T, TSelect> select)
        {
            using var d = Get();
            return d.QueryAsync<T>().Where(expression).ToEnumerable().Select(select).Distinct();
        }

        bool Retry()
        {
            return true;
        }


        void Db(Action<IDatabase> action) => DbGet(db =>
        {
            action(db);
            return true;
        });

        T DbGet<T>(Func<IDatabase, T> action)
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
                    throw new DataException("Data connection failed", exception);
                    //Thread.Sleep(5000);
                }
            }
        }

        async Task DbAsync(Func<IDatabase, Task> action) => await DbGetAsync(async db =>
         {
             await action(db);
             return true;
         });

        async Task<T> DbGetAsync<T>(Func<IDatabase, Task<T>> action)
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
                    throw new DataException("Data connection failed", exception);
                }
            }
        }

        Func<string> _getConnectionString = null;

        public void SetConfigureAction(Func<string> action)
        {            
            _getConnectionString = action;
            ServiceState = ServiceState.Available;
        }

        bool Configure(Exception exception = null)
        {
            return true;
        }

        public async IAsyncEnumerable<string> GetDatabasesAsync(string host, string login, string password)
        {
            var connString = $"Host={host};Username={login};Password={password};Database=postgres";

            await using var conn = new NpgsqlConnection(connString);
            await conn.OpenAsync().ConfigureAwait(false);
            List<string> databases = new List<string>();
            // Retrieve all rows
            await using (var cmd = new NpgsqlCommand("SELECT datname FROM pg_database;", conn))
            await using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    var database = reader.GetString(0);
                    databases.Add(database);
                }

            foreach (var database in databases)
            {
                if (!await IsHLabDatabasesAsync(host, database, login, password).ConfigureAwait(false)) continue;

                Debug.WriteLine($"Database : {database}");
                yield return database;
            }

            await conn.CloseAsync().ConfigureAwait(false);
        }
        public async Task<bool> IsHLabDatabasesAsync(string host, string database, string login, string password)
        {
            var connectionString = $"Host={host};Username={login};Password={password};Database={database}";

            await using var connection = new NpgsqlConnection(connectionString);

            // Retrieve all rows
            try
            {
                await connection.OpenAsync();
                await using var cmd =
                    new NpgsqlCommand($"SELECT \"Name\",\"Version\" FROM public.\"Module\";", connection);
                await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    var module = reader.GetString(0);
                    var version = reader.GetString(1);
                    if (module == "HLab.Erp") return true;
                }
            }
            catch (PostgresException e)

            {
                Debug.WriteLine(e.Message);
            }
            finally
            { 
                await connection.CloseAsync().ConfigureAwait(false);
            }
            
            Debug.WriteLine(database);

            return false;
        }



        public ServiceState ServiceState { get; private set; }
    }
}

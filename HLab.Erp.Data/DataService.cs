using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using HLab.DependencyInjection.Annotations;
using HLab.Base;
using NPoco;
using NPoco.Linq;

namespace HLab.Erp.Data
{

    [Export(typeof(IDataService)), Singleton]
    public class DataService : IDataService
    {
        public async Task<T> Add<T>(Action<T> setter, Action<T> added = null)
            where T : class, IEntity
        {
            using var db = Get();

            var t = (T)Activator.CreateInstance(typeof(T));  //_entityFactory(typeof(T));

            setter?.Invoke(t);

            object e = null;
            if (typeof(T).GetCustomAttributes<SoftIncrementAttribut>().FirstOrDefault() is SoftIncrementAttribut a)
            {
                if (t is IEntity<int> ti)
                {
                    var ids = await db.QueryAsync<T>().OrderByDescending(d => ((IEntity<int>)d).Id).FirstOrDefault();

                    var id = ((IEntity<int>)ids)?.Id ?? 0;

                    id++;

                    ti.Id = id;
                }
            }

            e = await db.InsertAsync<T>(t);

            if (e != null)
            {
                t.IsLoaded = true;
                added?.Invoke(t);
            }

            return await GetCache<T>().GetOrAdd(t);
        }
        public int Delete<T>(T entity, Action<T> deleted = null)
            where T : class, IEntity
        {
            int result = 0;
            using (var db = Get())
            {
                try
                {
                    db.Delete<T>(entity);

                    //result = db.SaveChanges();

                    GetCache<T>().Forget(entity);
                }
                catch (Exception e)
                {
                }
            }

            if (result > 0) deleted?.Invoke((T)entity);

            return result;
        }
        public async Task<T> GetOrAdd<T>(Expression<Func<T, bool>> getter, Action<T> setter, Action<T> added = null)
            where T : class, IEntity
        {
            var cache = GetCache<T>();
            T t;
            using (var db = Get())
            {
                t = await db.QueryAsync<T>().FirstOrDefault(getter);
            }

            if (t == null) return await Add(setter, added);

            return await cache.GetOrAdd(t);
        }
        public async Task<T> GetOrAdd<T>(T entity)
            where T : class, IEntity
        {
            return await GetCache<T>().GetOrAdd(entity);
        }
        //public static IQueryProviderWithIncludes<T> Query<T>() => D.Get().Query<T>();
        public async Task<List<T>> Fetch<T>() where T : class, IEntity
        {
            var cache = GetCache<T>();

            using var db = Get();
            var listIn = await db.FetchAsync<T>();

            var listOut = new List<T>();

            listIn.ToList().ForEach(async e => listOut.Add(await cache.GetOrAdd(e)));
            return listOut;
        }

        public void Execute(Action<IDatabase> action)
        {
            using var db = Get();
            action(db);
        }

//        public List<T> FetchQuery<T>(Func<IQueryable<T>, IQueryable<T>> q)
        public async Task<List<T>> FetchWhere<T>(Expression<Func<T, bool>> expression)
            where T : class, IEntity
        {
            var cache = GetCache<T>();
            if (typeof(ILocalCache).IsAssignableFrom(typeof(T)))
            {
                return await cache.Fetch(expression);
            }

            var list = await Get().QueryAsync<T>().Where(expression).ToList();

            return await cache.GetOrAdd(list);
        }

        public async Task<T> FetchOne<T>(Expression<Func<T, bool>> expression)
            where T : class, IEntity
        {
            using (var db = Get())
            {
                //TODO : connection Timeout
                //var result = db.FirstOrDefault<T>(expression);
                var result = await db.QueryAsync<T>().Where(expression).FirstOrDefault();
                return result == null ? null : await GetCache<T>().GetOrAdd(result);
            }
        }

        public async Task<T> FetchOne<T>(int id)
            where T : class, IEntity<int>

            => await FetchOne<T>((object)id);

        public async Task<T> FetchOne<T>(string id)
            where T : class, IEntity<string>

            => await FetchOne<T>((object)id);

        public async Task<T> FetchOne<T>(object id)
            where T : class, IEntity
        {
            var subscribe = false;

            var obj = await GetCache<T>().GetOrAdd(id,
                 async k =>
                {
                    subscribe = true;
                    var o = await Get().SingleByIdAsync<T>(k);
                    return o;
                });

            //if(subscribe && obj is INotifierObject nobj) nobj.GetNotifier().Subscribe();
            if (subscribe && obj is IEntity entity) entity.OnLoaded();
            if (subscribe && obj is IDataProvider dbf) dbf.DataService = this;

            return obj;

        }

        public bool Any<T>(Expression<Func<T, bool>> expression)
            where T : class, IEntity
        {
            using (var db = Get())
            {
                return db.Query<T>().Any(expression);
            }
        }

        private readonly ConcurrentDictionary<Type, DataCache> _caches = new ConcurrentDictionary<Type, DataCache>();

        private DataCache<T> GetCache<T>() where T : class, IEntity
            => (DataCache<T>)_caches.GetOrAdd(typeof(T), t => new DataCache<T>() { DataService = this });


        public string ConnectionString { get; private set; }
        private string _password;
        private string _driver;
        private int _nbOpen = 0;

        [Import]
        public Func<Type, object> _Locate { get; }
        public object Locate<T>(DbDataReader d) => _Locate(typeof(T));

        private IExportLocatorScope _container;

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

        public void Register(string connectionString, string driver)
        {
            ConnectionString = connectionString;
            _driver = driver;
        }

        public DbTransaction BeginTransaction() => Get().Transaction;
    }
}

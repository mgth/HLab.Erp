using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HLab.DependencyInjection.Annotations;
using HLab.Base;
using NPoco;
using NPoco.Linq;

namespace HLab.Erp.Data
{

    [Export(typeof(IDataService)), Singleton]
    public class DataService : IDataService
    {
        public T Add<T>(Action<T> setter, Action<T> added = null)
            where T : class, IEntity
        {
            using var db = Get();

            var t = (T)Activator.CreateInstance(typeof(T));  //_entityFactory(typeof(T));

            setter?.Invoke(t);

            object e = null;
            if (typeof(T).GetCustomAttributes<SoftIncrementAttribut>().FirstOrDefault() is SoftIncrementAttribut a)
            {
                //TODO efcore
                if (t is IEntity<int> ti)
                {
                    var ids = db.Query<T>().OrderByDescending(d => ((IEntity<int>)d).Id);

                    var id = ((IEntity<int>)ids.FirstOrDefault())?.Id ?? 0;

                    id++;

                    ti.Id = id;
                }
            }

            e = db.Insert<T>(t);

            if (e != null)
            {
                t.IsLoaded = true;
                added?.Invoke(t);
            }

            return GetCache<T>().GetOrAdd(t);
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
        public T GetOrAdd<T>(Expression<Func<T, bool>> getter, Action<T> setter, Action<T> added = null)
            where T : class, IEntity
        {
            var cache = GetCache<T>();
            T t;
            using (var db = Get())
            {
                t = db.Query<T>().FirstOrDefault(getter);
            }

            if (t == null) return Add(setter, added);

            return cache.GetOrAdd(t);
        }
        public T GetOrAdd<T>(T entity)
            where T : class, IEntity
        {
            return GetCache<T>().GetOrAdd(entity);
        }
        //public static IQueryProviderWithIncludes<T> Query<T>() => D.Get().Query<T>();
        public List<T> Fetch<T>() where T : class, IEntity
        {
            var cache = GetCache<T>();

            using var db = Get();
            var listIn = db.Fetch<T>();

            var listOut = new List<T>();

            listIn.ToList().ForEach(e => listOut.Add(cache.GetOrAdd(e)));
            return listOut;
        }

        public void Execute(Action<IDatabase> action)
        {
            using var db = Get();
            action(db);
        }

        public List<T> Fetch<T>(Func<IDatabase, List<T>> f)
            where T : class, IEntity
        {
            var cache = GetCache<T>();
            using var db = Get();
            var listIn = f(db);
            var listOut = new List<T>();

            listIn.ForEach(e => listOut.Add(cache.GetOrAdd(e)));
            return listOut;
        }
//        public List<T> FetchQuery<T>(Func<IQueryable<T>, IQueryable<T>> q)
        public List<T> FetchQuery<T>(Func<IQueryProviderWithIncludes<T>, IQueryProvider<T>> q)
            where T : class, IEntity
        {
            return Fetch(db => q(db.Query<T>()).ToList());
        }
        public List<T> FetchWhere<T>(Expression<Func<T, bool>> expression)
            where T : class, IEntity
        {
            if (typeof(ILocalCache).IsAssignableFrom(typeof(T)))
            {
                var cache = GetCache<T>();
                return cache.Fetch(expression);
            }
            return Fetch(db => db.Query<T>().Where(expression).ToList());
        }

        public T FetchOne<T>(Expression<Func<T, bool>> expression)
            where T : class, IEntity
        {
            using (var db = Get())
            {
                //TODO : connection Timeout
                //var result = db.FirstOrDefault<T>(expression);
                var result = db.Query<T>().Where(expression).FirstOrDefault();
                return result == null ? null : GetCache<T>().GetOrAdd(result);
            }
        }

        public T FetchOne<T>(int id)
            where T : class, IEntity<int>

            => GetCache<T>().Get(id);

        public T FetchOne<T>(string id)
            where T : class, IEntity<string>

            => GetCache<T>().Get(id);

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
                DatabaseType.MySQL,
                MySql.Data.MySqlClient.MySqlClientFactory.Instance
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

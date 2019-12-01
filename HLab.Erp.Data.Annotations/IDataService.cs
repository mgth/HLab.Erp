using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HLab.DependencyInjection.Annotations;

namespace HLab.Erp.Data
{
    public interface IDataService //: IService
    {

        //T Get<T>();
        void Register(string connectionString, string driver);
        bool Any<T>(Expression<Func<T, bool>> expression)
            where T : class, IEntity;

        Task<T> AddAsync<T>(Action<T> setter, Action<T> added = null) where T : class, IEntity;
        T Add<T>(Action<T> setter, Action<T> added = null) where T : class, IEntity;

        void Save<T>(T value) where T : class, IEntity;

        void Update<T>(T value, IEnumerable<string> columns) where T : class, IEntity;

        T FetchOne<T>(Expression<Func<T, bool>> expression) where T : class, IEntity;
        Task<T> FetchOneAsync<T>(Expression<Func<T, bool>> expression) where T : class, IEntity;
        Task<T> FetchOne<T>(int id) where T : class, IEntity<int>;
        Task<T> FetchOne<T>(string id) where T : class, IEntity<string>;
        Task<T> ReFetchOne<T>(T entity) where T : class, IEntity;
        Task<T> GetOrAdd<T>(Expression<Func<T, bool>> getter, Action<T> setter, Action<T> added = null) where T : class, IEntity;
        Task<T> GetOrAdd<T>(T entity) where T : class, IEntity;

        Task<List<T>> FetchWhereAsync<T>(
            Expression<Func<T, bool>> expression,
            Expression<Func<T,object>> orderBy
            ) where T : class, IEntity;

        int Delete<T>(T entity, Action<T> deleted = null)
            where T : class, IEntity;

        //void Execute(Action<IDatabase> action);
        Task<List<T>> Fetch<T>() where T : class, IEntity;

        void RegisterEntities(IExportLocatorScope container);
        List<Type> Entities { get; }

        string ConnectionString { get; }

        DbTransaction BeginTransaction();
    }
}
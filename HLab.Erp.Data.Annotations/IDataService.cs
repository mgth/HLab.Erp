using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;

namespace HLab.Erp.Data
{
    public interface IFluentTransaction
    {
        IFluentTransaction Add<T>(Action<T> setter, Action<T> added = null) where T : class, IEntity;

    }


    public interface IDataService : IService
    {
        IDataTransaction GetTransaction();
        //T Get<T>();
        bool Any<T>(Expression<Func<T, bool>> expression)
            where T : class, IEntity;
        T Add<T>(Action<T> setter, Action<T> added = null) where T : class, IEntity;
        Task<T> AddAsync<T>(Action<T> setter, Action<T> added = null) where T : class, IEntity;

        void Save<T>(T value) where T : class, IEntity;
        Task SaveAsync<T>(T value) where T : class, IEntity;

        bool Update<T>(T value, params string[] columns) where T : class, IEntity;
        Task<bool> UpdateAsync<T>(T value, params string[] columns) where T : class, IEntity;

        bool Update<T>(T value, Action<T> setter) where T : class, IEntity;
        Task<bool> UpdateAsync<T>(T value, Action<T> setter) where T : class, IEntity;

        T FetchOne<T>(Expression<Func<T, bool>> expression) where T : class, IEntity;
        Task<T> FetchOneAsync<T>(Expression<Func<T, bool>> expression) where T : class, IEntity;
        Task<T> FetchOneAsync<T>(int id) where T : class, IEntity<int>;
        Task<T> FetchOneAsync<T>(string id) where T : class, IEntity<string>;
        Task<T> ReFetchOneAsync<T>(T entity) where T : class, IEntity;

        T GetOrAdd<T>(Expression<Func<T, bool>> getter, Action<T> setter, Action<T> added = null) where T : class, IEntity;
        Task<T> GetOrAddAsync<T>(Expression<Func<T, bool>> getter, Action<T> setter, Action<T> added = null) where T : class, IEntity;
        Task<T> GetOrAddAsync<T>(T entity) where T : class, IEntity;

        IAsyncEnumerable<T> FetchWhereAsync<T>(
            Expression<Func<T, bool>> expression,
            Expression<Func<T,object>> orderBy = null
            ) where T : class, IEntity;

        IAsyncEnumerable<TSelect> SelectDistinctAsync<T,TSelect>(
            Expression<Func<T, bool>> expression,
            Func<T,TSelect> select
        );

        bool Delete<T>(T entity, Action<T> deleted = null)
            where T : class, IEntity;
        Task<bool> DeleteAsync<T>(T entity, Action<T> deleted = null)
            where T : class, IEntity;

        //void Execute(Action<IDatabase> action);
        IAsyncEnumerable<T> FetchAsync<T>() where T : class, IEntity;

        void RegisterEntities(IExportLocatorScope container);
        List<Type> Entities { get; }

        string ConnectionString { get; }
        string Source { get; set; }
        IEnumerable<string> Connections { get; }

        //DbTransaction BeginTransaction();
        void SetConfigureAction(Func<string> action);

        IAsyncEnumerable<string> GetDatabasesAsync(string host, string login, string password);
    }
}
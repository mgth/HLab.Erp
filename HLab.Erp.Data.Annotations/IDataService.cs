using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HLab.Base.Fluent;
using HLab.Core.Annotations;

namespace HLab.Erp.Data
{
    internal class Test : IEntity<int>
    {
        object IEntity.Id { get; }
        public int Id { get; set; }
        public bool IsLoaded { get; set; }
        public void OnLoaded()
        {
            throw new NotImplementedException();
        }
    }

    public class AddFluentHelper<TProvider> where TProvider : IDataProvider
    {
        TProvider _provider;

        public AddFluentHelper(TProvider provider)
        {
            _provider = provider;
        }

        public TProvider Entity<T>(Action<T> setter, Action<T> added = null) where T : class, IEntity
            => _provider.Add<TProvider,T>(setter, added);
    }

    public static class DataProviderExtension
    {
        public static TProvider Add<TProvider,T>(this TProvider data, Action<T> setter, Action<T> added = null)
            where T : class, IEntity
            where TProvider : IDataProvider
            => data.Fluently(d => d.Add(setter, added));



        public static AddFluentHelper<TProvider> Add<TProvider>(this TProvider data) where TProvider : IDataProvider
            => new(data);
    }


    public interface IDataProvider
    {
        T Add<T>(Action<T> setter, Action<T> added = null) where T : class, IEntity;
        Task<T> AddAsync<T>(Action<T> setter, Action<T> added = null) where T : class, IEntity;

        void Save<T>(T value) where T : class, IEntity;
        Task SaveAsync<T>(T value) where T : class, IEntity;

        bool Update<T>(T value, params string[] columns) where T : class, IEntity;
        Task<bool> UpdateAsync<T>(T value, params string[] columns) where T : class, IEntity;

        bool Update<T>(T value, Action<T> setter) where T : class, IEntity;
        Task<bool> UpdateAsync<T>(T value, Action<T> setter) where T : class, IEntity;

        bool Delete<T>(T entity, Action<T> deleted = null)
            where T : class, IEntity;
        Task<bool> DeleteAsync<T>(T entity, Action<T> deleted = null)
            where T : class, IEntity;
    }


    public interface IDataService : IService, IDataProvider
    {
        IDataTransaction GetTransaction();
        //T Get<T>();
        bool Any<T>(Expression<Func<T, bool>> expression)
            where T : class, IEntity;



        T FetchOne<T>(Expression<Func<T, bool>> expression) where T : class, IEntity;
        Task<T> FetchOneAsync<T>(Expression<Func<T, bool>> expression) where T : class, IEntity;
        Task<T> FetchOneAsync<T>(int id) where T : class, IEntity<int>;
        Task<T> FetchOneAsync<T>(string id) where T : class, IEntity<string>;
        Task<T> ReFetchOneAsync<T>(T entity) where T : class, IEntity;

        T GetOrAdd<T>(Expression<Func<T, bool>> getter, Action<T> setter, Action<T> added = null) where T : class, IEntity;
        Task<T> GetOrAddAsync<T>(Expression<Func<T, bool>> getter, Action<T> setter, Action<T> added = null) where T : class, IEntity;
        Task<T> GetOrAddAsync<T>(T entity) where T : class, IEntity;

        IEnumerable<T> FetchWhere<T>(
            Expression<Func<T, bool>> expression,
            Expression<Func<T, object>>? orderBy = null
        ) where T : class, IEntity;

        IAsyncEnumerable<T> FetchWhereAsync<T>(
            Expression<Func<T, bool>> expression,
            Expression<Func<T, object>>? orderBy = null
            ) where T : class, IEntity;

        IAsyncEnumerable<TSelect> SelectDistinctAsync<T,TSelect>(
            Expression<Func<T, bool>> expression,
            Func<T,TSelect> select
        );


        //void Execute(Action<IDatabase> action);
        IAsyncEnumerable<T> FetchAsync<T>() where T : class, IEntity;

        List<Type> Entities { get; }

        string ConnectionString { get; }
        string Source { get; set; }
        IEnumerable<string> Connections { get; }

        //DbTransaction BeginTransaction();
        void SetConfigureAction(Func<Task<string>> action);

        IAsyncEnumerable<string> GetDatabasesAsync(string host, string login, string password);
    }
}
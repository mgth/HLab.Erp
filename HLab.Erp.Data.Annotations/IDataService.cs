using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using HLab.DependencyInjection.Annotations;

namespace HLab.Erp.Data
{
    public interface IDataService //: IService
    {

        //T Get<T>();
        void Register(string connectionString, string driver);
        bool Any<T>(Expression<Func<T, bool>> expression)
            where T : class, IEntity;
        T Add<T>(Action<T> setter, Action<T> added = null) where T : class, IEntity;

        T FetchOne<T>(Expression<Func<T, bool>> expression) where T : class, IEntity;
        T FetchOne<T>(int id) where T : class, IEntity<int>;
        T FetchOne<T>(string id) where T : class, IEntity<string>;

        T GetOrAdd<T>(Expression<Func<T, bool>> getter, Action<T> setter, Action<T> added = null) where T : class, IEntity;
        T GetOrAdd<T>(T entity) where T : class, IEntity;

        List<T> FetchWhere<T>(Expression<Func<T, bool>> expression) where T : class, IEntity;
        //List<T> FetchQuery<T>(Func<IQueryable<T>, IQueryable<T>> q) where T : class, IEntity;
        //List<T> FetchQuery<T>(Func<IQueryProviderWithIncludes<T>, IQueryProvider<T>> q)
        int Delete<T>(T entity, Action<T> deleted = null)
            where T : class, IEntity;

        //void Execute(Action<IDatabase> action);
        List<T> Fetch<T>() where T : class, IEntity;

        void RegisterEntities(IExportLocatorScope container);
        List<Type> Entities { get; }

        string ConnectionString { get; }

        DbTransaction BeginTransaction();
    }
}
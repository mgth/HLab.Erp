using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HLab.Erp.Data
{
    public interface IDataTransaction : IDisposable
    {
        T Add<T>(Action<T> setter, Action<T> added = null) where T : class, IEntity;
        Task<T> AddAsync<T>(Action<T> setter, Action<T> added = null) where T : class, IEntity;

        void Update<T>(T value, IEnumerable<string> columns) where T : class, IEntity;
        Task<bool> UpdateAsync<T>(T value, IEnumerable<string> columns) where T : class, IEntity;

        void Save<T>(T value) where T : class, IEntity;
        Task SaveAsync<T>(T value) where T : class, IEntity;

        int Delete<T>(T entity, Action<T> deleted = null)
            where T : class, IEntity;
        Task<int> DeleteAsync<T>(T entity, Action<T> deleted = null)
            where T : class, IEntity;

        void Done();
    }
}
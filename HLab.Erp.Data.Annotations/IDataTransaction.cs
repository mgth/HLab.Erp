using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HLab.Erp.Data
{
    public interface IDataTransaction : IDataProvider, IDisposable
    {
        void Update<T>(T value, IEnumerable<string> columns) where T : class, IEntity;
        Task<bool> UpdateAsync<T>(T value, IEnumerable<string> columns) where T : class, IEntity;

        void Done();
        void ExecuteSql(string sql);
    }
}